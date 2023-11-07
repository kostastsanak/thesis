using energy_service.Core;
using energy_service.Handlers;
using lib_energy;
using lib_energy_dao.DBModels;
using Microsoft.AspNetCore.Mvc;
using ActionResponse = energy_service.Core.ActionResponse;

namespace energy_service.Controllers
{
    [ApiController]
    [Route("api/gov/electricity/production")]
    public class ElectricityProductionController : ControllerBase
    {
        private readonly ActionResponse _response;
        private readonly energy_DBHandling _dBHandling;
        private readonly ElectricityProductionHandler _electricityProductionHandler;

        public ElectricityProductionController(IConfiguration configuration)
        {
            _response = new ActionResponse();
            _dBHandling = new energy_DBHandling(configuration);
            _electricityProductionHandler = new ElectricityProductionHandler(configuration, _response);
        }

        [HttpPut("update")]
        public async Task<ActionResponse> PassDataToDatabase()
        {
            try
            {
                var data = await _electricityProductionHandler.GetData();
                if (!data.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "An error occurred at Electrical production data");
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


        [HttpPost("search")]
        public ActionResponse SearchDataFromDatabase(ElectricityProduction.Search search)
        {
            try
            {
                var data = _electricityProductionHandler.SearchData(search);
                if (data == null)
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "An error occurred at search");
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


        [HttpGet("fuels")]
        public ActionResponse GetFuels()
        {
            try
            {
                var data = _dBHandling.GetDBFuels().ToList();
                if (!data.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "An error occurred at search");
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


    }
}