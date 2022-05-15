using System.Net;

namespace DominoesProperties.Models
{
    public class ApiResponse
    {
        public bool Success;
        public string Message;
        public object Data { get; set; }

        public ApiResponse(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }
    }
}
