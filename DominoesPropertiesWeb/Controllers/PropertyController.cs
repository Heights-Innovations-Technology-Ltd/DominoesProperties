using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

        [HttpGet("/Property/viewproperty/{uniqueid}")]
        public IActionResult ViewProperty()
        {
            return View();
        }

        [HttpGet("/Property/edit/{uniqueid}")]
        public IActionResult Edit()
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
            obj.CreatedBy = Convert.ToString(jObject["CreatedBy"]);

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
            return Json(JsonConvert.SerializeObject(data));
        }
        
        [Route("update-property/{propertyId}")]
        public async Task<JsonResult> EditProperty([FromBody] dynamic property, string propertyId)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(property));
            
            dynamic obj = new ExpandoObject();

            obj.Location = Convert.ToString(jObject["Location"]);
            obj.Type = Convert.ToInt32(jObject["Type"]);
            obj.UnitPrice = Convert.ToInt32(jObject["UnitPrice"]);
            obj.ClosingDate = Convert.ToInt32(jObject["ClosingDate"]);
            obj.TotalUnits = Convert.ToInt32(jObject["UnitAvailable"]);
            obj.InterestRate = Convert.ToInt32(jObject["InterestRate"]);
            obj.Longitude = Convert.ToString(jObject["Longitude"]);
            obj.Latitude = Convert.ToString(jObject["Latitude"]);
            //obj.TargetYield = jObject["TargetYield"] != null ? Convert.ToDecimal(jObject["TargetYield"]) : 0;
            //obj.ProjectedGrowth = jObject["ProjectedGrowth"] != null ? Convert.ToDecimal(jObject["ProjectedGrowth"]) : 0;
            obj.Summary = Convert.ToString(jObject["Summary"]);

            var res = Task.Run(() => httpContext.Put("Property/" + propertyId, obj));
            var data = await res.GetAwaiter().GetResult();
            return Json(JsonConvert.SerializeObject(data));
        }
        
        [Route("property-decription/{propertyId}")]
        public async Task<JsonResult> PropertyDescription([FromBody] dynamic property, string propertyId)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(property));
            dynamic DesObj = new ExpandoObject();

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


            var res = Task.Run(() => httpContext.Put("Property/description/" + propertyId, DesObj));
            var data = await res.GetAwaiter().GetResult();
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
        
        [Route("single-property/{uniqueId}")]
        public async Task<JsonResult> GetSingleProperty([FromRoute] string uniqueId)
        {
            var res = Task.Run(() => httpContext.Get("Property/" + uniqueId));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [HttpPost("/upload-property/{propertyId}")]
        public async Task<JsonResult> uploadDoc(string propertyId)
        {
            var json = Request.Form.Files;
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();
            
            foreach (var file in json)
            {
                var fileContent = new StreamContent(Request.Body);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = file.Name,
                    FileName = file.FileName
                };
                content.Add(fileContent);


            }
            //var requestUri = "Property/uploads/" + propertyId;
            //client.BaseAddress = new Uri("https://localhost:44361/api/");
            //var result = client.PostAsync(requestUri, content).Result;
            //Console.WriteLine($"Response : {result.StatusCode}");

            if (json.Count > 0)
            {

                var res = Task.Run(() => httpContext.PostUpload("Property/uploads/" + propertyId, content));
                var data = await res.GetAwaiter().GetResult();
                return Json(JsonConvert.SerializeObject(data));
            }
            return Json(this.StatusCode(StatusCodes.Status204NoContent, "Empty request body"));
        }

        private List<string> GetUploadedFileName(HttpRequest httpRequest, string propertyId)
        {
            List<string> filenamelistofCustomerFiles = new();
            foreach (var formfile in httpRequest.Form.Files)
            {
                if (formfile.Length > 0)
                {
                    string filePath = Path.Combine(hostEnvironment.WebRootPath, _config["app_settings:commodityUploads"]);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    using (var stream = new FileStream(Path.Combine(filePath, propertyId + formfile.FileName), FileMode.Create))
                    {
                        formfile.CopyTo(stream);
                    }
                    filenamelistofCustomerFiles.Add(formfile.FileName);
                }
            }
            return filenamelistofCustomerFiles;
        }
    }
}
