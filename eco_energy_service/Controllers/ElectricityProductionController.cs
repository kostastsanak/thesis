using eco_energy_services.Handlers;
using eco_lib_core;
using eco_lib_energy;
using eco_lib_energy_dao.DBModels;
using Microsoft.AspNetCore.Mvc;

namespace eco_energy_services.Controllers
{
    [ApiController]
    [Route("api/gov/electricity/production")]
    public class ElectricityProductionController : ControllerBase
    {
        private ActionResponse _response;
        private readonly string _recipient;
        private readonly energy_DBHandling _dBHandling;
        private readonly ElectricityProductionHandler _electricityProductionHandler;

        public ElectricityProductionController(IConfiguration configuration)
        {
            _response = new ActionResponse();
            _dBHandling = new energy_DBHandling(configuration);
            _recipient = configuration.GetValue<string>("GlobalVariables:ERROR_EMAIL_NOTIFICATIONS");
            _electricityProductionHandler = new ElectricityProductionHandler(configuration, _response);
        }

        [HttpPut("update")]
        public async Task<ActionResponse> PassDatatoDatabase()
        {
            try
            {
                var data = await _electricityProductionHandler.GetData();
                if (!data.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.INSERT, "An error occurred at Electrical production data");
                }
                else
                {
                    ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.INSERT, "Electrical production data updated successfully!", data);
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
        public ActionResponse SearchDatafromDatabase(ElectricityProduction.Search Search)
        {
            try
            {
                var data = _electricityProductionHandler.SearchData(Search);
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


        [HttpGet("fuels")]
        public ActionResponse GetFuels()
        {
            try
            {
                var data = _dBHandling.GetDBFuels().ToList();
                if (!data.IsListSafe())
                {
                    ActionResponseHandlers.HandleError(ref _response, operationType: ActionResponseHandlers.OperationType.FETCH, "An error occurred at search");
                }
                else
                {
                    ActionResponseHandlers.HandleSuccess(ref _response, ActionResponseHandlers.OperationType.FETCH, "Fuel results retrieved successfully!", data);
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