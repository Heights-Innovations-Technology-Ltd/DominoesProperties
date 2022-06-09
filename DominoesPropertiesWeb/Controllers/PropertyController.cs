using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Models;
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
        public async Task<JsonResult> CreateProperty([FromBody] dynamic property)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(property));
            
            dynamic obj = new ExpandoObject();
            dynamic DesObj = new ExpandoObject();

            obj.Name = Convert.ToString(jObject["Name"]);
            obj.Location = Convert.ToString(jObject["Location"]);
            obj.Type = Convert.ToInt32(jObject["Type"]);
            obj.UnitPrice = Convert.ToInt32(jObject["UnitPrice"]);
            obj.Status = Convert.ToInt32(jObject["Status"]);
            obj.UnitAvailable = Convert.ToInt32(jObject["UnitAvailable"]);
            obj.InterestRate = Convert.ToInt32(jObject["InterestRate"]);
            obj.Longitude = Convert.ToString(jObject["Longitude"]);
            obj.Latitude = Convert.ToString(jObject["Latitude"]);
            obj.CreatedBy = "afolabihabeeb1@outlook.com";

            DesObj.Bathroom = Convert.ToInt32(jObject["Description"]["Bathroom"]);
            DesObj.Toilet = Convert.ToInt32(jObject["Description"]["Toilet"]);
            DesObj.FloorLevel = Convert.ToInt32(jObject["Description"]["FloorLevel"]);
            DesObj.Bedroom = Convert.ToInt32(jObject["Description"]["Bedroom"]);
            DesObj.LandSize = Convert.ToString(jObject["Description"]["LandSize"]);
            DesObj.AirConditioned = Convert.ToInt32(jObject["Description"]["AirConditioned"]);
            DesObj.Refrigerator = Convert.ToInt32(jObject["Description"]["Refrigerator"]);
            DesObj.Parking = Convert.ToInt32(jObject["Description"]["Parking"]);
            DesObj.SwimmingPool = Convert.ToInt32(jObject["Description"]["SwimmingPool"]);
            DesObj.Laundry = Convert.ToInt32(jObject["Description"]["Laundry"]);
            DesObj.Gym = Convert.ToInt32(jObject["Description"]["Gym"]);
            DesObj.SecurityGuard = Convert.ToInt32(jObject["Description"]["SecurityGuard"]);
            DesObj.Fireplace = Convert.ToInt32(jObject["Description"]["Fireplace"]);
            DesObj.Basement = Convert.ToInt32(jObject["Description"]["Basement"]);
            obj.Description = DesObj;


            var res = Task.Run(() => httpContext.Post("Property", obj));
            var data = await res.GetAwaiter().GetResult();
            
            //await Task.WhenAll(res);
            //var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
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

        [Route("get-property-types")]
        public async Task<JsonResult> GetPropertyType()
        {
            var res = Task.Run(() => httpContext.Get("Property/types"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
    }
}
