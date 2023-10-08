using System.Text.Json;
using System.Text.Json.Serialization;
using eco_energy_services.Core;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using MySql.Data.MySqlClient;
using ActionResponse = eco_energy_services.Core.ActionResponse;

namespace eco_energy_services.Handlers
{
    public class RenewableElectricityHandler
    {
        private const string Url = "https://data.gov.gr/api/v1/query/admie_realtimescadares";//?date_from=2023-07-07&date_to=2023-07-14
        private const string Token = "14f9afc3ffb341d9e0c22c2807f6e4c1a60f75cd";

        private readonly IConfiguration _config;
        private readonly energy_DBHandling _dBHandling;
        private readonly ActionResponse _response;
        private readonly int _limit;
        private readonly JsonSerializerOptions _options;
        public RenewableElectricityHandler(IConfiguration configuration, ActionResponse response)
        {
            _config = configuration;
            //_httpClient = new HttpClient();

            _dBHandling = new energy_DBHandling(_config);
            _response = response;//new ActionResponse();
            _limit = _config.GetValue<int>("GlobalVariables:DEFAULT_ENTRIES_PER_PAGE");

            _options = new JsonSerializerOptions 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }
        #region URL
        public async Task<List<RenewableElectricity.Response>?> GetRenewableElectricityUrl()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token "+Token+"");
            var response = await client.GetAsync(Url);
            if (!response.IsSuccessStatusCode) { return null; }

            var responseContent = response.Content;
            var responseString = await responseContent.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<List<RenewableElectricity.Response>>(responseString, _options);

            return result;
        }

        #endregion

        #region Tools

        public string CreateQuery(RenewableElectricity.Search search)
        {
            var queryList = new List<string>();
            
            if (search.Year != null)
            {
                queryList.Add("year=\"" + search.Year + "\" ");
            }

            if (search.Month != null)
            {
                queryList.Add("month=\"" + search.Month + "\" ");
            }
            if (search.Day != null)
            {
                queryList.Add("day=\"" + search.Day + "\" ");
            }

            return string.Join("&&", queryList);
        }

        public async Task SaveRenewableElectricity(List<DBRenewableElectricity> toBePassed, string conString)
        {
            var toBeDeleted = _dBHandling.GetDBRenewableElectricities().ToList();
            _dBHandling.DeleteDBRenewableElectricities(toBeDeleted);
            _dBHandling.CommitToDb();

            var commandText = "ALTER TABLE renewable_electricity AUTO_INCREMENT = 1";
            await using (var connection = new MySqlConnection(conString))
            {
                var command = new MySqlCommand(commandText, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            _dBHandling.UpdateDBRenewableElectricities(toBePassed);
            _dBHandling.CommitToDb();
        }

        public void FixOrderBy(ref List<DBRenewableElectricity> tobefetched, RenewableElectricity.Search search)
        {
            if (search.GroupByYear == 1)
            {
                tobefetched = tobefetched.GroupBy(x => new { x.Year })
                    .Select(g => new DBRenewableElectricity
                    {
                        Year = g.Key.Year,

                        Month = 1,
                        Day = 1,
                        Energy_mwh = g.Sum(x => x.Energy_mwh)
                    })
                    .ToList();
            }
            if (search.GroupByMonth == 1)
            {
                tobefetched = tobefetched.GroupBy(x => new { x.Year, x.Month })
                    .Select(g => new DBRenewableElectricity
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = 1,
                        Energy_mwh = g.Sum(x => x.Energy_mwh)
                    })
                    .ToList();
            }
        }


        #endregion


        public async Task<List<DBRenewableElectricity>> GetData() // Completion time - 650 ms
        {
            try
            {
                var conString = _config.GetValue<string>("ConnectionStrings:DBContext");

                var renewableElectricity = await GetRenewableElectricityUrl();

                //var toBePassed = new List<DBRenewableElectricity>().RemapFrom(something);
                //var toBePassed = (List<DBRenewableElectricity>)Remaper.remapObjects<DBRenewableElectricity>(renewableElectricity, new List<DBRenewableElectricity>());
                var toBePassed = RemapFromResponse(renewableElectricity);

                toBePassed.ForEach(e =>
                {
                    e.Year = e.Date.Year;
                    e.Month = e.Date.Month;
                    e.Day = e.Date.Day;
                });
                var dailyData = toBePassed
                    .GroupBy(x => x.Date.Date)
                    .Select(g => new DBRenewableElectricity
                    {
                        Date = g.Key,
                        Energy_mwh = g.Sum(x => x.Energy_mwh),
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day
                    }).ToList();


                await SaveRenewableElectricity(dailyData, conString);

                return dailyData.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
                throw;
            }
        }

        public RenewableElectricity.ApiEnergyPagging? SearchData(RenewableElectricity.Search? search) // Completion time - 188 ms
        {
            try
            {
                if (search == null)
                {
                    return null;
                }
                var payload = new RenewableElectricity.ApiEnergyPagging();
                var query = CreateQuery(search);
                var toBeFetched = _dBHandling.GetDBRenewableElectricities(query).ToList();
                if (!toBeFetched.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "No such Data");
                    return payload;
                }
                FixOrderBy(ref toBeFetched, search);

                payload.Pagging = Utilities.CalculatePagging(search.Page, toBeFetched.Count, search.Limit, _limit, noPagging: Convert.ToBoolean(search.NoPagging));

                if (payload.Pagging.CurrentPage <= payload.Pagging.TotalPages)
                {
                    var renewableElectricity = new List<RenewableElectricity.Response>();
                    var tempResults = toBeFetched.Skip(payload.Pagging.EntriesStart).Take(payload.Pagging.EntriesPerPage).ToList();
                    tempResults.ForEach(ep =>
                    {
                        renewableElectricity.Add(new RenewableElectricity.Response
                        {
                            Energy_mwh = ep.Energy_mwh,
                            Date = new DateTime(ep.Year, ep.Month, ep.Day)
                        });
                    });

                    payload.RenewableElectricity = renewableElectricity;
                }
                else
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "No Results, requested Page " + payload.Pagging.CurrentPage + " is greater than total number of pages " + payload.Pagging.TotalPages);
                }
                return payload;
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
                throw;
            }
        }

        public List<DBRenewableElectricity> RemapFromResponse(List<RenewableElectricity.Response> tempResults)
        {
            return tempResults.Select(temp => new DBRenewableElectricity { Date = temp.Date, Energy_mwh = temp.Energy_mwh }).ToList();
        }

    }

}
