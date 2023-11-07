using System.Runtime.CompilerServices;

namespace energy_service.Core
{
    public static class ActionResponseHandler
    {
        public static void CreateResponse(ActionResponse response , string status, object? payload, [CallerMemberName] string methodTitle = "")
        {
            response.Message = methodTitle;
            response.Status = status;
            response.Payload = payload;
        }
    }
}
