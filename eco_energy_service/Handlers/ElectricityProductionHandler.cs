using System.Text.Json;
using System.Text.Json.Serialization;
using eco_lib_core;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using MySql.Data.MySqlClient;

namespace eco_energy_services.Handlers
{
    public class ElectricityProductionHandler
    {
        private const string _Url = "https://data.gov.gr/api/v1/query/admie_dailyenergybalanceanalysis";//?date_from=2023-07-07&date_to=2023-07-14
        private const string _Token = "14f9afc3ffb341d9e0c22c2807f6e4c1a60f75cd";

        private readonly IConfiguration _config;
        private readonly energy_DBHandling _dBHandling;
        private ActionResponse _response;
        private readonly int _limit;
        private readonly string _recipient;
        private readonly JsonSerializerOptions _options;
        public ElectricityProductionHandler(IConfiguration configuration, ActionResponse response)
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

        public string CreateQuery(ElectricityProduction.Search Search)
        {
            var queryList = new List<string>();
            if (Search.FuelIDs.IsListSafe())
            {
                queryList.Add("FuelID IN (" + string.Join(',',Search.FuelIDs) + ")");
            }
            if (Search.Fuel != null)
            {
                queryList.Add("Fuel.StartsWith(\"" + Search.Fuel.ToUpper() + "\")");
            }
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

        public async Task SaveElectricalProduction(List<DBElectricityProduction> tobepassed, string con_string)
        {
            try
            {
                var tobeDeleted = _dBHandling.GetDBElectricityProductions().ToList();
                _dBHandling.DeleteDBElectricityProductions(tobeDeleted);
                _dBHandling.CommitToDb();

                var commandText = "ALTER TABLE electricity_production AUTO_INCREMENT = 1";
                await using (var connection = new MySqlConnection(con_string))
                {
                    var command = new MySqlCommand(commandText, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }

                _dBHandling.UpdateDBElectricityProductions(tobepassed);
                _dBHandling.CommitToDb();

            }
            catch (Exception ex)
            {
                throw (ex ?? ex.InnerException)!;
            }
        }

        public void FixOrderBy(ref List<DBElectricityProduction> tobefetched, ElectricityProduction.Search search)
        {
            if (search.GroupByFuel == 0 || search.GroupByFuel == null)
            {
                if (search.GroupByYear == 1)
                {
                    tobefetched = tobefetched.GroupBy(x => new { x.Year })
                        .Select(g => new DBElectricityProduction
                        {
                            Year = g.Key.Year,
                            
                            Month = 1,
                            Day = 1,
                            Fuel = null,
                            Energy_mwh = g.Sum(x => x.Energy_mwh)
                        })
                        .ToList();
                }
                if (search.GroupByMonth == 1)
                {
                    tobefetched = tobefetched.GroupBy(x => new { x.Year, x.Month })
                        .Select(g => new DBElectricityProduction
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Day = 1,
                            Fuel = null,
                            Energy_mwh = g.Sum(x => x.Energy_mwh)
                        })
                        .ToList();
                }
            }
            else
            {
                var fuels = _dBHandling.GetDBFuels();
                if ((search.GroupByYear == 0 || search.GroupByYear == null) && (search.GroupByMonth == 0 || search.GroupByMonth == null))
                {
                    tobefetched = tobefetched.GroupBy(x => new {x.FuelID })
                        .Select(g => new DBElectricityProduction
                        {
                            Year = 1,
                            FuelID = g.Key.FuelID,
                            Fuel = fuels.Where(f => g.Key.FuelID == f.FuelID).Select(f => f.FuelValue).First(),
                            Month = 1,
                            Day = 1,
                            Energy_mwh = g.Sum(x => x.Energy_mwh)
                        })
                        .ToList();
                }

                if (search.GroupByYear == 1)
                {
                    tobefetched = tobefetched.GroupBy(x => new { x.Year, x.FuelID })
                        .Select(g => new DBElectricityProduction
                        {
                            Year = g.Key.Year,
                            FuelID = g.Key.FuelID,
                            Fuel = fuels.Where(f=> g.Key.FuelID == f.FuelID).Select(f=>f.FuelValue).First(),
                            Month = 1,
                            Day = 1,
                            Energy_mwh = g.Sum(x => x.Energy_mwh)
                        })
                        .ToList();
                }
                if (search.GroupByMonth == 1)
                {
                    tobefetched = tobefetched.GroupBy(x => new { x.Year, x.Month, x.FuelID })
                        .Select(g => new DBElectricityProduction
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Day = 1,
                            FuelID = g.Key.FuelID,
                            Fuel = fuels.Where(f => g.Key.FuelID == f.FuelID).Select(f => f.FuelValue).First(),
                            Energy_mwh = g.Sum(x => x.Energy_mwh)
                        })
                        .ToList();
                }
            }

        }


        #endregion

        #region URL
        public async Task<List<ElectricityProduction.Response>> GetElectricityProductionURL()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token "+_Token+"");
            var response = await client.GetAsync(_Url);
            if (!response.IsSuccessStatusCode) { return null; }

            var responseContent = response.Content;
            var responseString = await responseContent.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<ElectricityProduction.Response>>(responseString, _options);

            return result;
        }

        #endregion

        public async Task<List<DBElectricityProduction>> GetData() // Completion time - 188 ms
        {
            try
            {
                var con_string = _config.GetValue<string>("ConnectionStrings:DBContext");

                var something = await GetElectricityProductionURL();

                var tobepassed = new List<DBElectricityProduction>().RemapFrom(something);
                var fuels = _dBHandling.GetDBFuels().ToList();
                tobepassed.ForEach(e =>
                {
                    e.FuelID = fuels.Where(f => f.FuelValue == e.Fuel).Select(f=>f.FuelID).FirstOrDefault();
                    e.Year = e.Date.Year;
                    e.Month = e.Date.Month;
                    e.Day = e.Date.Day;
                });


                await SaveElectricalProduction(tobepassed, con_string);

                return tobepassed.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex ?? ex.InnerException)!;
            }
        }
        
        public ElectricityProduction.ApiEnergyPagging? SearchData(ElectricityProduction.Search? search) // Completion time - 188 ms
        {
            try
            {
                if (search == null)
                {
                    return null;
                }
                
                var payload = new ElectricityProduction.ApiEnergyPagging();
                
                var query = CreateQuery(search);
                var tobefetched = _dBHandling.GetDBElectricityProductions(query).ToList();
                if (!tobefetched.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "No such Data");
                    return payload;
                }
                FixOrderBy(ref tobefetched, search);

                payload.Pagging = Utilities.calcuatePagging(search.Page, tobefetched.Count, search.Limit, _limit, noPagging: Convert.ToBoolean(search.NoPagging));

                if (payload.Pagging.CurrentPage <= payload.Pagging.TotalPages)
                {
                    var electrical_productions = new List<ElectricityProduction.Response>();
                    var tempResults = tobefetched.Skip(payload.Pagging.EntriesStart).Take(payload.Pagging.EntriesPerPage).ToList();
                    tempResults.ForEach(ep => 
                    {
                        electrical_productions.Add(new ElectricityProduction.Response
                        {
                            Fuel = ep.Fuel,
                            Energy_mwh = ep.Energy_mwh,
                            Date = new DateTime(ep.Year, ep.Month, ep.Day)
                        });
                    });

                    payload.Productions = electrical_productions;
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
