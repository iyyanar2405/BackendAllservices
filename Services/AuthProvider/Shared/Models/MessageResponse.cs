namespace Hadvida.EmailService.Models
{
    public class MessageResponse
    {
        public MessageResponse(string statusCode, string msg="", string to = "") { 
            StatusCode = statusCode;
            Error = msg;
            To = to;
        }
        public string StatusCode { get; set; }

        public string? Error { get; set; }
        public string? To { get; set; }
    }
}
