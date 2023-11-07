using energy_service.Core;
using energy_service.Handlers;
using lib_energy;
using Microsoft.AspNetCore.Mvc;
using ActionResponse = energy_service.Core.ActionResponse;

namespace energy_service.Controllers
{
    [ApiController]
    [Route("api/gov/renewable/electricity/production")]
    public class RenewableElectricityController : ControllerBase
    {
        private readonly ActionResponse _response;
        private readonly RenewableElectricityHandler _renewableElectricityHandler;

        public RenewableElectricityController(IConfiguration configuration)
        {
            _response = new ActionResponse();
            _renewableElectricityHandler = new RenewableElectricityHandler(configuration, _response);
        }

        [HttpPut("update")]
        public async Task<ActionResponse> PassDataToDatabase()
        {
            try
            {
                var data = await _renewableElectricityHandler.GetData();
                if (!data.IsListSafe())
                {
                    ActionResponseHandler.CreateResponse(_response, "ERROR", "An error occurred at renewable electricity data");
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
        public ActionResponse SearchDataFromDatabase(RenewableElectricity.Search search)
        {
            try
            {
                var data = _renewableElectricityHandler.SearchData(search);
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


    }
}