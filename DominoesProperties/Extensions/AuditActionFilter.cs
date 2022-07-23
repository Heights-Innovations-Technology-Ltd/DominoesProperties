//using System;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Repositories.Repository;

//namespace DominoesProperties.Extensions
//{
//    public class AuditActionFilter : IAsyncActionFilter
//    {
//        private readonly ILoggerManager logger;

//        public AuditActionFilter(ILoggerManager _logger)
//        {
//            logger = _logger;
//        }

//        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            var request = context.HttpContext.Request;

//            Portal_Logger audit = new Portal_Logger()
//            {
//                userid = context.HttpContext.User.Identity.Name,
//                IPAddress = Convert.ToString(ipHostInfo.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)),
//                ControllerAccess = (string)context.RouteData.Values["controller"],
//                Timestamp = DateTime.Now,
//            };

//            logger.LogInfo($"[{audit.Timestamp}]: {audit.IPAddress} : [{audit.ControllerAccess}:{audit.userid}] => ");
//            //await FormatRequest(request);
//            await next();
//            //var response = await FormatResponse(context.HttpContext.Response);
//        }

//        private async Task<string> FormatRequest(HttpRequest request)
//        {
//            var body = request.Body;

//            //This line allows us to set the reader for the request back at the beginning of its stream.
//            request.EnableBuffering();

//            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
//            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

//            //...Then we copy the entire request stream into the new buffer.
//            await request.Body.ReadAsync(buffer, 0, buffer.Length);

//            //We convert the byte[] into a string using UTF8 encoding...
//            var bodyAsText = Encoding.UTF8.GetString(buffer);

//            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
//            request.Body = body;

//            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
//        }

//        private async Task<string> FormatResponse(HttpResponse response)
//        {
//            //We need to read the response stream from the beginning...
//            response.Body.Seek(0, SeekOrigin.Begin);

//            //...and copy it into a string
//            string text = await new StreamReader(response.Body).ReadToEndAsync();

//            //We need to reset the reader for the response so that the client can read it.
//            response.Body.Seek(0, SeekOrigin.Begin);

//            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
//            return $"{response.StatusCode}: {text}";
//        }
//    }

//    public class Portal_Logger
//    {
//        public int LoggerId { get; set; }

//        public string userid { get; set; }

//        public string IPAddress { get; set; }

//        public string ControllerAccess { get; set; }

//        public DateTime? Timestamp { get; set; }

//    }
//}

