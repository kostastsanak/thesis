using eco_energy_services.Handlers;
using eco_lib_core;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace eco_energy_services.Controllers
{
    [ApiController]
    [Route("api/gov/electricity/consumption")]
    public class ElectricityConsumptionController : ControllerBase
    {
        private ActionResponse _response;
        private readonly string _recipient;
        private readonly ElectricityConsumptionHandler _electricityConsumptionHandler;
        private readonly energy_DBHandling _dBHandling;

        public ElectricityConsumptionController(IConfiguration configuration)
        {
            _dBHandling = new energy_DBHandling(configuration);
            _response = new ActionResponse();
            _recipient = configuration.GetValue<string>("GlobalVariables:ERROR_EMAIL_NOTIFICATIONS");
            _electricityConsumptionHandler = new ElectricityConsumptionHandler(configuration, _response);
        }

        [HttpPut("update")]
        public async Task<ActionResponse> PassDatatoDatabase()
        {
            try
            {
                var data = await _electricityConsumptionHandler.GetData();
                if (!data.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.INSERT, "An error occurred at Electrical consumption data");
                }
                else
                {
                    ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.INSERT, "Electrical consumption data updated successfully!", data);
                }
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex??ex.InnerException)!;
            }

            return _response;
        }

        [HttpGet("cities")]
        public async Task<ActionResponse> Cities()
        {
            try
            {
                var data = _dBHandling.GetDBCities().ToList();
                if (!data.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "An error occurred at cities retrieve");
                }
                else
                {
                    ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.INSERT, "Cities retrieved successfully!", data);
                }
            }
            catch (Exception ex)
            {
                ActionResponseHandlers.HandleException(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, ex: ex, recipient: _recipient);
                throw (ex ?? ex.InnerException)!;
            }

            return _response;
        }

        [HttpPost("search")]
        public ActionResponse SearchDatafromDatabase(ElectricityConsumption.Search Search)
        {
            try
            {
                ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.INSERT, "Search results retrieved successfully!", _electricityConsumptionHandler.SearchData(Search));
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