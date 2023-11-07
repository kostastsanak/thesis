using System.Text.Json;
using System.Text.Json.Serialization;
using energy_service.Core;
using lib_energy;
using lib_energy_dao.DBModels;
using MySql.Data.MySqlClient;
using ActionResponse = energy_service.Core.ActionResponse;

namespace energy_service.Handlers
{
    public class ElectricityProductionHandler
    {
        private const string Url = "https://data.gov.gr/api/v1/query/admie_dailyenergybalanceanalysis";//?date_from=2023-07-07&date_to=2023-07-14
        private const string Token = "14f9afc3ffb341d9e0c22c2807f6e4c1a60f75cd";

        private readonly IConfiguration _config;
        private readonly energy_DBHandling _dBHandling;
        private readonly ActionResponse _response;
        private readonly int _limit;
        private readonly JsonSerializerOptions _options;
        public ElectricityProductionHandler(IConfiguration configuration, ActionResponse response)
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

        public string CreateQuery(ElectricityProduction.Search search)
        {
            var queryList = new List<string>();
            if (search.FuelIDs.IsListSafe())
            {
                queryList.Add("FuelID IN (" + string.Join(',',search.FuelIDs) + ")");
            }
            if (search.Fuel != null)
            {
                queryList.Add("Fuel.StartsWith(\"" + search.Fuel.ToUpper() + "\")");
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

        public async Task SaveElectricalProduction(List<DBElectricityProduction> toBePassed, string conString)
        {
            var toBeDeleted = _dBHandling.GetDBElectricityProductions().ToList();
            _dBHandling.DeleteDBElectricityProductions(toBeDeleted);
            _dBHandling.CommitToDb();

            var commandText = "ALTER TABLE electricity_production AUTO_INCREMENT = 1";
            await using (var connection = new MySqlConnection(conString))
            {
                var command = new MySqlCommand(commandText, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            _dBHandling.UpdateDBElectricityProductions(toBePassed);
            _dBHandling.CommitToDb();
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
                            Fuel = null!,
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
                            Fuel = null!,
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
        public async Task<List<ElectricityProduction.Response>?> GetElectricityProductionUrl()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token "+Token+"");
            var response = await client.GetAsync(Url);
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
                var conString = _config.GetValue<string>("ConnectionStrings:DBContext");

                var electricityProductions = await GetElectricityProductionUrl();
                
                //var toBePassed = new List<DBElectricityProduction>().RemapFrom(something);
                //var toBePassed = (List<DBElectricityProduction>)Remaper.remapObjects<DBElectricityProduction>(electricityProductions, new List<DBElectricityProduction>());
                var toBePassed = RemapFromResponse(electricityProductions);


                var fuels = _dBHandling.GetDBFuels().ToList();
                toBePassed.ForEach(e =>
                {
                    e.FuelID = fuels.Where(f => f.FuelValue == e.Fuel).Select(f=>f.FuelID).FirstOrDefault();
                    e.Year = e.Date.Year;
                    e.Month = e.Date.Month;
                    e.Day = e.Date.Day;
                });


                await SaveElectricalProduction(toBePassed, conString);

                return toBePassed.Take(10).ToList();
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
                throw;
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
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "No such Data");
                    return payload;
                }
                FixOrderBy(ref tobefetched, search);

                payload.Pagging = Utilities.CalculatePagging(search.Page, tobefetched.Count, search.Limit, _limit, noPagging: Convert.ToBoolean(search.NoPagging));

                if (payload.Pagging.CurrentPage <= payload.Pagging.TotalPages)
                {
                    var electricalProductions = new List<ElectricityProduction.Response>();
                    var tempResults = tobefetched.Skip(payload.Pagging.EntriesStart).Take(payload.Pagging.EntriesPerPage).ToList();
                    tempResults.ForEach(ep => 
                    {
                        electricalProductions.Add(new ElectricityProduction.Response
                        {
                            Fuel = ep.Fuel,
                            Energy_mwh = ep.Energy_mwh,
                            Date = new DateTime(ep.Year, ep.Month, ep.Day)
                        });
                    });

                    payload.Productions = electricalProductions;
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

        public List<DBElectricityProduction> RemapFromResponse(List<ElectricityProduction.Response> tempResults)
        {
            return tempResults.Select(temp => new DBElectricityProduction { Fuel = temp.Fuel, Date = temp.Date, Energy_mwh = temp.Energy_mwh}).ToList();
        }

    }

}
