using System.Text.Json;
using System.Text.Json.Serialization;
using eco_lib_core;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace eco_energy_services.Handlers
{
    public class ElectricityConsumptionHandler
    {
        //private const string _Production = "https://data.gov.gr/api/v1/query/admie_dailyenergybalanceanalysis?date_from=2023-07-06&date_to=2023-07-13";
        private const string _Url = "https://data.gov.gr/api/v1/query/electricity_consumption";
        private const string _Token = "14f9afc3ffb341d9e0c22c2807f6e4c1a60f75cd";

        private readonly IConfiguration _config;
        private readonly energy_DBHandling _dBHandling;
        private ActionResponse _response;
        private readonly int _limit;
        private readonly string _recipient;
        private readonly JsonSerializerOptions _options;
        public ElectricityConsumptionHandler(IConfiguration configuration, ActionResponse response)
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

        public async Task SaveElectricalConsumption(List<DBElectricityConsumption> tobepassed, string con_string)
        {
            try
            {
                var tobeDeleted = _dBHandling.GetDBElectricityConsumptions().ToList();
                _dBHandling.DeleteDBElectricityConsumptions(tobeDeleted);
                _dBHandling.CommitToDb();

                var commandText = "ALTER TABLE electricity_consumption AUTO_INCREMENT = 1";
                await using (var connection = new MySqlConnection(con_string))
                {
                    var command = new MySqlCommand(commandText, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }

                _dBHandling.UpdateDBElectricityConsumptions(tobepassed);
                _dBHandling.CommitToDb();

            }
            catch (Exception ex)
            {
                throw (ex ?? ex.InnerException)!;
            }
        }

        public async Task SaveCountries(List<string?> countries, string con_string)
        {
            try
            {
                var dbdelcities = _dBHandling.GetDBCities().ToList();
                _dBHandling.DeleteDBCities(dbdelcities);
                _dBHandling.CommitToDb();
                var commandText = "ALTER TABLE city AUTO_INCREMENT = 1";
                await using (var connection = new MySqlConnection(con_string))
                {
                    var command = new MySqlCommand(commandText, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }

                var dbcities = countries.Select(country => new DBCity { CityName = country }).ToList();

                _dBHandling.UpdateDBCities(dbcities);
                _dBHandling.CommitToDb();

            }
            catch (Exception ex)
            {
                throw (ex ?? ex.InnerException)!;
            }
        }

        #endregion

        #region URL
        public async Task<List<ElectricityConsumption.Response>> GetElectricityConsumptionURL()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token "+_Token+"");
            var response = await client.GetAsync(_Url);
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

                var dbElecticityConsumption = await GetElectricityConsumptionURL();

                var tobepassed = new List<DBElectricityConsumption>().RemapFrom(dbElecticityConsumption);

                var city = _dBHandling.GetDBCities().ToList();
                tobepassed.ForEach(e =>
                {
                    e.Area = e.Area?.Trim();
                    e.CityID = city.Where(c => c.CityName == e.Area).Select(c => c.CityID).FirstOrDefault();
                    e.Year = e.Date.Year;
                    e.Month = e.Date.Month;
                    e.Day = e.Date.Day;
                });

                var countries = tobepassed.Select(t => t.Area).Distinct().ToList();

                await SaveCountries(countries, con_string);
                await SaveElectricalConsumption(tobepassed, con_string);

                return tobepassed.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex ?? ex.InnerException)!;
            }
        }

        public ElectricityConsumption.ApiEnergyPagging SearchData(ElectricityConsumption.Search search) // Completion time - 188 ms
        {
            try
            {
                var payload = new ElectricityConsumption.ApiEnergyPagging();
                var query = CreateQuery(search);
                var tobefetched = _dBHandling.GetDBElectricityConsumptions(query).OrderByDescending(g => g.Date).ToList();

                if(!tobefetched.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "No such Data");
                    return payload;
                }
                payload.Pagging = Utilities.calcuatePagging(search.Page, tobefetched.Count, search.Limit, _limit, noPagging: Convert.ToBoolean(search.NoPagging));

                if (payload.Pagging.CurrentPage <= payload.Pagging.TotalPages)
                {
                    if (search.GroupByCity == 1)
                    {
                        tobefetched = tobefetched.GroupBy(x => new { x.Area})
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
                    var tempResults = tobefetched.Skip(payload.Pagging.EntriesStart).Take(payload.Pagging.EntriesPerPage).ToList();
                    tempResults.ForEach(temp =>
                    {
                        temp.Energy_mwh = Math.Round(temp.Energy_mwh, 2);
                    });
                    var electrical_consumptions = new List<ElectricityConsumption.Response>().RemapFrom(tempResults);

                    payload.Consumptions = electrical_consumptions;
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
