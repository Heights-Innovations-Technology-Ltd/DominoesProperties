using System.Net;

namespace DominoesPropertiesWeb.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public object Data { get; set; }

    }
}
