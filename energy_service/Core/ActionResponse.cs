namespace energy_service.Core
{
    public class ActionResponse
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public List<rbe_message> Messages { get; set; } = new List<rbe_message>();
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object Payload { get; set; }
        public ActionResponse()
        {
            Timestamp = DateTime.Now;
            Status = "OK";
            Message = "";
        }
        public static ActionResponse Create(String Status, String Message)
        {
            ActionResponse response = new ActionResponse();
            response.Status = Status;
            response.Message = Message;
            return response;
        }
        public static ActionResponse Create(String Status, String Message, Object Object)
        {
            ActionResponse response = new ActionResponse();
            response.Status = Status;
            response.Message = Message;
            response.Payload = Object;
            return response;
        }
    }

    public class rbe_message
    {
        public int? MessageType { get; set; }
        public string MessageCode { get; set; }
        public string Content { get; set; }
    }
}