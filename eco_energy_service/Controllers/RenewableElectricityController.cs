using eco_energy_services.Handlers;
using eco_lib_core;
using eco_lib_energy;
using Microsoft.AspNetCore.Mvc;

namespace eco_energy_services.Controllers
{
    [ApiController]
    [Route("api/gov/renewable/electricity/production")]
    public class RenewableElectricityController : ControllerBase
    {
        private ActionResponse _response;
        private readonly string _recipient;
        private readonly RenewableElectricityHandler _renewableElectricityHandler;

        public RenewableElectricityController(IConfiguration configuration)
        {
            _response = new ActionResponse();
            _recipient = configuration.GetValue<string>("GlobalVariables:ERROR_EMAIL_NOTIFICATIONS");
            _renewableElectricityHandler = new RenewableElectricityHandler(configuration, _response);
        }

        [HttpPut("update")]
        public async Task<ActionResponse> PassDatatoDatabase()
        {
            try
            {
                var data = await _renewableElectricityHandler.GetData();
                if (!data.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.INSERT, "An error occurred at renewable electricity data");
                }
                else
                {
                    ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.INSERT, "renewable electricity data updated successfully!", data);
                }
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex??ex.InnerException)!;
            }

            return _response;
        }
        
        [HttpPost("search")]
        public ActionResponse SearchDatafromDatabase(RenewableElectricity.Search Search)
        {
            try
            {
                var data = _renewableElectricityHandler.SearchData(Search);
                if (data == null)
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "An error occurred at search");
                }
                else
                {
                    ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.INSERT, "Search results retrieved successfully!", data);
                }
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex ?? ex.InnerException)!;
            }

            return _response;
        }


    }
}