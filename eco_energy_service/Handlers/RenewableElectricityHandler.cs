using System.Text.Json;
using System.Text.Json.Serialization;
using eco_lib_core;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using MySql.Data.MySqlClient;

namespace eco_energy_services.Handlers
{
    public class RenewableElectricityHandler
    {
        private const string _Url = "https://data.gov.gr/api/v1/query/admie_realtimescadares";//?date_from=2023-07-07&date_to=2023-07-14
        private const string _Token = "14f9afc3ffb341d9e0c22c2807f6e4c1a60f75cd";

        private readonly IConfiguration _config;
        private readonly energy_DBHandling _dBHandling;
        private ActionResponse _response;
        private readonly string _recipient;
        private readonly int _limit;
        private readonly JsonSerializerOptions _options;
        public RenewableElectricityHandler(IConfiguration configuration, ActionResponse response)
        {
            _config = configuration;
            //_httpClient = new HttpClient();

            _dBHandling = new energy_DBHandling(_config);
            _response = response;//new ActionResponse();
            _recipient = _config.GetValue<string>("GlobalVariables:ERROR_EMAIL_NOTIFICATIONS");
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
        public async Task<List<RenewableElectricity.Response>> GetRenewableElectricityURL()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token "+_Token+"");
            var response = await client.GetAsync(_Url);
            if (!response.IsSuccessStatusCode) { return null; }

            var responseContent = response.Content;
            var responseString = await responseContent.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<List<RenewableElectricity.Response>>(responseString, _options);

            return result;
        }

        #endregion

        #region Tools

        public string CreateQuery(RenewableElectricity.Search Search)
        {
            var queryList = new List<string>();
            
            if (Search.Year != null)
            {
                queryList.Add("year=\"" + Search.Year + "\" ");
            }

            if (Search.Month != null)
            {
                queryList.Add("month=\"" + Search.Month + "\" ");
            }
            if (Search.Day != null)
            {
                queryList.Add("day=\"" + Search.Day + "\" ");
            }

            return string.Join("&&", queryList);
        }

        public async Task SaveRenewableElectricity(List<DBRenewableElectricity> tobepassed, string con_string)
        {
            try
            {
                var tobeDeleted = _dBHandling.GetDBRenewableElectricities().ToList();
                _dBHandling.DeleteDBRenewableElectricities(tobeDeleted);
                _dBHandling.CommitToDb();

                var commandText = "ALTER TABLE renewable_electricity AUTO_INCREMENT = 1";
                await using (var connection = new MySqlConnection(con_string))
                {
                    var command = new MySqlCommand(commandText, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }

                _dBHandling.UpdateDBRenewableElectricities(tobepassed);
                _dBHandling.CommitToDb();

            }
            catch (Exception ex)
            {
                throw (ex ?? ex.InnerException)!;
            }
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
                var con_string = _config.GetValue<string>("ConnectionStrings:DBContext");

                var something = await GetRenewableElectricityURL();

                var tobepassed = new List<DBRenewableElectricity>().RemapFrom(something);

                tobepassed.ForEach(e =>
                {
                    e.Year = e.Date.Year;
                    e.Month = e.Date.Month;
                    e.Day = e.Date.Day;
                });
                var dailyData = tobepassed
                    .GroupBy(x => x.Date.Date)
                    .Select(g => new DBRenewableElectricity
                    {
                        Date = g.Key,
                        Energy_mwh = g.Sum(x => x.Energy_mwh),
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Day = g.Key.Day
                    }).ToList();


                await SaveRenewableElectricity(dailyData, con_string);

                return dailyData.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex ?? ex.InnerException)!;
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
                var tobefetched = _dBHandling.GetDBRenewableElectricities(query).ToList();
                if (!tobefetched.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "No such Data");
                    return payload;
                }
                FixOrderBy(ref tobefetched, search);

                payload.Pagging = Utilities.calcuatePagging(search.Page, tobefetched.Count, search.Limit, _limit, noPagging: Convert.ToBoolean(search.NoPagging));

                if (payload.Pagging.CurrentPage <= payload.Pagging.TotalPages)
                {
                    var renewable_electricity = new List<RenewableElectricity.Response>();
                    var tempResults = tobefetched.Skip(payload.Pagging.EntriesStart).Take(payload.Pagging.EntriesPerPage).ToList();
                    tempResults.ForEach(ep =>
                    {
                        renewable_electricity.Add(new RenewableElectricity.Response
                        {
                            Energy_mwh = ep.Energy_mwh,
                            Date = new DateTime(ep.Year, ep.Month, ep.Day)
                        });
                    });

                    payload.RenewableElectricity = renewable_electricity;
                }
                else
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "No Results, requested Page " + payload.Pagging.CurrentPage + " is greater than total number of pages " + payload.Pagging.TotalPages);
                }
                return payload;
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex ?? ex.InnerException)!;
            }
        }



    }

}
