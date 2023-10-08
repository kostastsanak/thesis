using eco_energy_services.Core;
using eco_energy_services.Handlers;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace eco_energy_services.Controllers
{
    [ApiController]
    [Route("api/gov/electricity/consumption")]
    public class ElectricityConsumptionController : ControllerBase
    {
        private readonly ActionResponse _response;
        private readonly ElectricityConsumptionHandler _electricityConsumptionHandler;
        private readonly energy_DBHandling _dBHandling;

        public ElectricityConsumptionController(IConfiguration configuration)
        {
            _dBHandling = new energy_DBHandling(configuration);
            _response = new ActionResponse();
            _electricityConsumptionHandler = new ElectricityConsumptionHandler(configuration, _response);
        }

        [HttpPut("update")]
        public async Task<ActionResponse> PassDataToDatabase()
        {
            try
            {
                var data = await _electricityConsumptionHandler.GetData();
                if (!data.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "An error occurred at Electrical consumption data");
                }
                else
                {
                    ActionResponseHandler.CreateResponse(_response, "SUCCESS", data);
                }
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
            }

            return _response;
        }

        [HttpGet("cities")]
        public Task<ActionResponse> Cities()
        {
            try
            {
                var data = _dBHandling.GetDBCities().ToList();
                if (!data.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "An error occurred at cities retrieve");
                }
                else
                {
                    ActionResponseHandler.CreateResponse(_response, "SUCCESS", data);
                }
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
            }

            return Task.FromResult(_response);
        }

        [HttpPost("search")]
        public ActionResponse SearchDataFromDatabase(ElectricityConsumption.Search search)
        {
            try
            {
                ActionResponseHandler.CreateResponse(_response, "SUCCESS", _electricityConsumptionHandler.SearchData(search));
            }
            catch (Exception ex)
            {
                ActionResponseHandler.CreateResponse(_response, "EXCEPTION", ex);
            }

            return _response;
        }


    }
}