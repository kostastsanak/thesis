using System.Text.Json;
using System.Text.Json.Serialization;
using energy_service.Core;
using lib_energy;
using lib_energy_dao.DBModels;
using MySql.Data.MySqlClient;
using ActionResponse = energy_service.Core.ActionResponse;

namespace energy_service.Handlers
{
    public class ElectricityConsumptionHandler
    {
        //private const string _Production = "https://data.gov.gr/api/v1/query/admie_dailyenergybalanceanalysis?date_from=2023-07-06&date_to=2023-07-13";
        private const string Url = "https://data.gov.gr/api/v1/query/electricity_consumption";
        private const string Token = "14f9afc3ffb341d9e0c22c2807f6e4c1a60f75cd";

        private readonly IConfiguration _config;
        private readonly energy_DBHandling _dBHandling;
        private readonly ActionResponse _response;
        private readonly int _limit;
        private readonly JsonSerializerOptions _options;
        public ElectricityConsumptionHandler(IConfiguration configuration, ActionResponse response)
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

        #region Tools

        public string CreateQuery(ElectricityConsumption.Search search)
        {
            var queryList = new List<string>();
            if (search.CityID != null)
            {
                queryList.Add("CityID=\"" + search.CityID + "\" ");
            }
            if (search.City != null)
            {
                queryList.Add("area.StartsWith(\"" + search.City.ToUpper() + "\")");
            }
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

        public async Task SaveElectricalConsumption(List<DBElectricityConsumption> toBePassed, string conString)
        {
            var toBeDeleted = _dBHandling.GetDBElectricityConsumptions().ToList();
            _dBHandling.DeleteDBElectricityConsumptions(toBeDeleted);
            _dBHandling.CommitToDb();

            var commandText = "ALTER TABLE electricity_consumption AUTO_INCREMENT = 1";
            await using (var connection = new MySqlConnection(conString))
            {
                var command = new MySqlCommand(commandText, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            _dBHandling.UpdateDBElectricityConsumptions(toBePassed);
            _dBHandling.CommitToDb();
        }

        public async Task SaveCountries(List<string?> countries, string conString)
        {
            var dbdDelCities = _dBHandling.GetDBCities().ToList();
            _dBHandling.DeleteDBCities(dbdDelCities);
            _dBHandling.CommitToDb();
            var commandText = "ALTER TABLE city AUTO_INCREMENT = 1";
            await using (var connection = new MySqlConnection(conString))
            {
                var command = new MySqlCommand(commandText, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            var dbCities = countries.Select(country => new DBCity { CityName = country }).ToList();

            _dBHandling.UpdateDBCities(dbCities);
            _dBHandling.CommitToDb();
        }

        #endregion

        #region URL
        public async Task<List<ElectricityConsumption.Response>?> GetElectricityConsumptionUrl()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token "+Token+"");
            var response = await client.GetAsync(Url);
            if (!response.IsSuccessStatusCode) { return null; }

            var responseContent = response.Content;
            var responseString = await responseContent.ReadAsStringAsync();

            

            var result = JsonSerializer.Deserialize<List<ElectricityConsumption.Response>>(responseString, _options);

            return result;
        }

        #endregion

        public async Task<List<DBElectricityConsumption>> GetData() // Completion time - 188 ms
        {
            try
            {
                var con_string = _config.GetValue<string>("ConnectionStrings:DBContext");

                var electricityConsumptions = await GetElectricityConsumptionUrl();

                //var toBePassed = new List<DBElectricityConsumption>().RemapFrom(electricityConsumption);
                //var toBePassed = (List<DBElectricityConsumption>)Remaper.remapObjects<DBElectricityConsumption>(electricityConsumptions, new List<DBElectricityConsumption>());
                var toBePassed = RemapFromResponse(electricityConsumptions);

                var city = _dBHandling.GetDBCities().ToList();
                toBePassed.ForEach(e =>
                {
                    e.Area = e.Area?.Trim();
                    e.CityID = city.Where(c => c.CityName == e.Area).Select(c => c.CityID).FirstOrDefault();
                    e.Year = e.Date.Year;
                    e.Month = e.Date.Month;
                    e.Day = e.Date.Day;
                });

                var countries = toBePassed.Select(t => t.Area).Distinct().ToList();

                await SaveCountries(countries, con_string);
                await SaveElectricalConsumption(toBePassed, con_string);

                return toBePassed.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
                throw;
            }
        }

        public ElectricityConsumption.ApiEnergyPagging SearchData(ElectricityConsumption.Search search) // Completion time - 188 ms
        {
            try
            {
                var payload = new ElectricityConsumption.ApiEnergyPagging();
                var query = CreateQuery(search);
                var toBeFetched = _dBHandling.GetDBElectricityConsumptions(query).OrderByDescending(g => g.Date).ToList();

                if(!toBeFetched.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "No such Data");
                    return payload;
                }
                payload.Pagging = Utilities.CalculatePagging(search.Page, toBeFetched.Count, search.Limit, _limit, noPagging: Convert.ToBoolean(search.NoPagging));

                if (payload.Pagging.CurrentPage <= payload.Pagging.TotalPages)
                {
                    if (search.GroupByCity == 1)
                    {
                        toBeFetched = toBeFetched.GroupBy(x => new { x.Area})
                            .Select(g => new DBElectricityConsumption
                            {
                                Area = g.Key.Area,
                                Year = 1,
                                Month = 1,
                                Day = 1,
                                Energy_mwh = g.Sum(x => x.Energy_mwh)
                            }).OrderByDescending(g=>g.Energy_mwh)
                            .ToList();
                    }
                    var tempResults = toBeFetched.Skip(payload.Pagging.EntriesStart).Take(payload.Pagging.EntriesPerPage).ToList();
                    tempResults.ForEach(temp =>
                    {
                        temp.Energy_mwh = Math.Round(temp.Energy_mwh, 2);
                    });
                    var electricalConsumptions = RemapToResponse (tempResults);


                    payload.Consumptions = electricalConsumptions;
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

        public List<DBElectricityConsumption> RemapFromResponse(List<ElectricityConsumption.Response> tempResults)
        {
            return tempResults.Select(temp => new DBElectricityConsumption { Area = temp.Area, Date = temp.Date, Energy_mwh = temp.Energy_mwh }).ToList();
        }

        public List<ElectricityConsumption.Response> RemapToResponse(List<DBElectricityConsumption> tempResults)
        {
            return tempResults.Select(temp => new ElectricityConsumption.Response { Area = temp.Area, Date = temp.Date, Energy_mwh = temp.Energy_mwh }).ToList();
        }


    }

}
