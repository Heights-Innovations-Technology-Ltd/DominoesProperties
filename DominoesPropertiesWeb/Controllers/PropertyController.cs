using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ISession session;

        private readonly IHttpContext httpContext;
        private readonly IWebHostEnvironment hostEnvironment;

        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public PropertyController(IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext httpContext, IWebHostEnvironment hostEnvironment)
        {
            _config = config;
            this.session = httpContextAccessor.HttpContext.Session;
            this.httpContext = httpContext;
            this.hostEnvironment = hostEnvironment;
            url = _config["Base_URL"];
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [Route("create-property")]
        public async Task<JsonResult> CreateProperty([FromBody] Property property)
        {
            var res = Task.Run(() => httpContext.Post("Customer/register", property));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("get-properties")]
        public async Task<JsonResult> GetProperties()
        {
            var res = Task.Run(() => httpContext.Get("Property"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
    }
}
