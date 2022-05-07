using System.Net;

namespace DominoesProperties.Models
{
    public class ApiResponse
    {
        public HttpStatusCode Code;
        public string Message;
        public object Data { get; set; }

        public ApiResponse(HttpStatusCode Code, string Message)
        {
            this.Code = Code;
            this.Message = Message;
        }
    }
}
