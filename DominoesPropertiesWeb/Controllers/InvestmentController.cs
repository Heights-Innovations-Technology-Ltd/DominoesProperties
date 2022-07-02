using DominoesPropertiesWeb.HttpContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.Controllers
{
    public class InvestmentController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ISession session;

        private readonly IHttpContext httpContext;

        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public InvestmentController(IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext httpContext)
        {
            _config = config;
            this.session = httpContextAccessor.HttpContext.Session;
            this.httpContext = httpContext;
            url = _config["Base_URL"];
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/invest")]
        public async Task<JsonResult> propertyInvestment([FromBody] dynamic property)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(property));

            dynamic obj = new ExpandoObject();

            obj.PropertyUniqueId = Convert.ToString(jObject["propertyUniqueId"]);
            obj.Units = 1;
            //obj.Units = Convert.ToInt32(jObject["units"]);
            var res = Task.Run(() => httpContext.Post("Investment", obj));
            var data = await res.GetAwaiter().GetResult();
            //await Task.WhenAll(res);
            //var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
    }
}
