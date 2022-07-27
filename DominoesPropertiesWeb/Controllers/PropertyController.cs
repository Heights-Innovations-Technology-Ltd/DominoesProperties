using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Models;
using DominoesPropertiesWeb.Repository;
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
        private readonly IUploadsRepository _uploadRepository;
        private readonly IWebHostEnvironment hostEnvironment;

        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public PropertyController(IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext httpContext, IWebHostEnvironment hostEnvironment, IUploadsRepository uploadRepository)
        {
            _config = config;
            this.session = httpContextAccessor.HttpContext.Session;
            this.httpContext = httpContext;
            this.hostEnvironment = hostEnvironment;
            url = _config["Base_URL"];
            _uploadRepository = uploadRepository;
        }
        public IActionResult Index()
        {
            var isAuthAdmin = this.session.GetString("RoleFK");
            if (isAuthAdmin == null || isAuthAdmin.Equals(string.Empty))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("/Property/viewproperty/{uniqueid}")]
        public IActionResult ViewProperty()
        {
            var isAuthAdmin = this.session.GetString("RoleFK");
            if (isAuthAdmin == null || isAuthAdmin.Equals(string.Empty))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet("/Property/edit/{uniqueid}")]
        public IActionResult Edit()
        {
            var isAuthAdmin = this.session.GetString("RoleFK");
            if (isAuthAdmin == null || isAuthAdmin.Equals(string.Empty))
            {
                return RedirectToAction("Index", "Home");
            }
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
            obj.TotalUnits = Convert.ToInt32(jObject["UnitAvailable"]);
            obj.InterestRate = Convert.ToInt32(jObject["InterestRate"]);
            obj.Longitude = Convert.ToString(jObject["Longitude"]);
            obj.Latitude = Convert.ToString(jObject["Latitude"]);
            obj.CreatedBy = Convert.ToString(jObject["CreatedBy"]);
            obj.AccountNumber = Convert.ToString(jObject["Account"]);
            obj.BankName = Convert.ToString(jObject["Bank"]);
            obj.MaxUnitPerCustomer = Convert.ToInt32(jObject["MaxUnitPerCustomer"]);
            obj.ClosingDate = Convert.ToDateTime(jObject["ClosingDate"]);
            obj.Summary = Convert.ToString(jObject["Summary"]);

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
            obj.TotalUnits = Convert.ToInt32(jObject["UnitAvailable"]);
            obj.InterestRate = Convert.ToInt32(jObject["InterestRate"]);
            obj.Longitude = Convert.ToString(jObject["Longitude"]);
            obj.Latitude = Convert.ToString(jObject["Latitude"]);
            //obj.TargetYield = jObject["TargetYield"] != null ? Convert.ToDecimal(jObject["TargetYield"]) : 0;
            //obj.ProjectedGrowth = jObject["ProjectedGrowth"] != null ? Convert.ToDecimal(jObject["ProjectedGrowth"]) : 0;
            obj.AccountNumber = Convert.ToString(jObject["Account"]);
            obj.BankName = Convert.ToString(jObject["Bank"]);
            obj.MaxUnitPerCustomer = Convert.ToInt32(jObject["MaxUnitPerCustomer"]);
            obj.ClosingDate = Convert.ToDateTime(jObject["ClosingDate"]);
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


        [Route("/filter-property")]
        public async Task<JsonResult> GetFilterProperty([FromBody] dynamic filter)
        {
            var query = new Dictionary<string, string>();

            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(filter));
            dynamic DesObj = new ExpandoObject();
            var propertyFilter = new PropertyFilter();
            //propertyFilter.Category = Convert.ToInt32(jObject["Category"]);
            propertyFilter.Bathroom = Convert.ToInt32(jObject["Bathroom"]);
            propertyFilter.Toilet = Convert.ToInt32(jObject["Toilet"]);
            propertyFilter.Floor = Convert.ToInt32(jObject["FloorLevel"]);
            propertyFilter.Bedroom = Convert.ToInt32(jObject["Bedroom"]);
            propertyFilter.MinPrice = Convert.ToDecimal(jObject["MinPrice"]);
            propertyFilter.MaxPrice = Convert.ToDecimal(jObject["MaxPrice"]);
            propertyFilter.AirConditioned = Convert.ToBoolean(jObject["AirConditioned"]);
            propertyFilter.Refridgerator = Convert.ToBoolean(jObject["Refrigerator"]);
            propertyFilter.Parking = Convert.ToBoolean(jObject["Parking"]);
            propertyFilter.SwimmingPool = Convert.ToBoolean(jObject["SwimmingPool"]);
            propertyFilter.Laundry = Convert.ToBoolean(jObject["Laundry"]);
            propertyFilter.Gym = Convert.ToBoolean(jObject["Gym"]);
            propertyFilter.SecurityGuard = Convert.ToBoolean(jObject["SecurityGuard"]);
            propertyFilter.Fireplace = Convert.ToBoolean(jObject["Fireplace"]);
            propertyFilter.Basement = Convert.ToBoolean(jObject["Basement"]);

            //if (Convert.ToInt32(jObject["Category"]) > 0) query.Add("Category", propertyFilter.Category.ToString());
            if (Convert.ToInt32(jObject["Bathroom"]) > 0) query.Add("Bathroom", propertyFilter.Bathroom.ToString());
            if (Convert.ToInt32(jObject["Toilet"]) > 0) query.Add("Toilet", propertyFilter.Toilet.ToString());
            if (Convert.ToInt32(jObject["FloorLevel"]) > 0) query.Add("Floor", propertyFilter.Floor.ToString());
            if (Convert.ToInt32(jObject["Bedroom"]) > 0) query.Add("Bedroom", propertyFilter.Bedroom.ToString());
            if (Convert.ToDecimal(jObject["MinPrice"]) > 0) query.Add("MinPrice", propertyFilter.MinPrice.ToString());
            if (Convert.ToDecimal(jObject["MaxPrice"]) > 0) query.Add("MaxPrice", propertyFilter.MaxPrice.ToString());
            if (Convert.ToBoolean(jObject["AirConditioned"])) query.Add("AirConditioned", propertyFilter.AirConditioned.ToString());
            if (Convert.ToBoolean(jObject["Refrigerator"])) query.Add("Refridgerator", propertyFilter.Refridgerator.ToString());
            if (Convert.ToBoolean(jObject["Parking"])) query.Add("Parking", propertyFilter.Parking.ToString());
            if (Convert.ToBoolean(jObject["SwimmingPool"])) query.Add("SwimmingPool", propertyFilter.SwimmingPool.ToString());
            if (Convert.ToBoolean(jObject["Laundry"])) query.Add("Laundry", propertyFilter.Laundry.ToString());
            if (Convert.ToBoolean(jObject["Gym"])) query.Add("Gym", propertyFilter.Gym.ToString());
            if (Convert.ToBoolean(jObject["SecurityGuard"])) query.Add("SecurityGuard", propertyFilter.SecurityGuard.ToString());
            if (Convert.ToBoolean(jObject["Fireplace"])) query.Add("Fireplace", propertyFilter.Fireplace.ToString());
            if (Convert.ToBoolean(jObject["Basement"])) query.Add("Basement", propertyFilter.Basement.ToString());

            var res = Task.Run(() => httpContext.Get("Property", query));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [HttpPost("/upload-property/{propertyId}")]
        public async Task<JsonResult> uploadDoc(string propertyId)
        {
            var obj = Request.Form["uploadType"];
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(obj));
            var json = Request.Form.Files;
            if (json.Count > 0)
            {
                //using var content = new MultipartFormDataContent();
                List<PropertyFileUpload> fileUploads = new List<PropertyFileUpload>();
                foreach (var file in json)
                {
                    #region old implementation
                    //var fileContent = new StreamContent(Request.Body);
                    //fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    //fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    //{
                    //    Name = file.Name,
                    //    FileName = file.FileName,
                    //    //UploadType = Convert.ToInt32(jObject[""])
                    //};
                    //content.Add(fileContent);
                    #endregion

                    var upload = new PropertyFileUpload
                    {
                        File = file,
                        UploadType = (UploadType)Convert.ToInt32(jObject["uploadType"])
                    };
                    fileUploads.Add(upload);

                }
                var uploadResponse = _uploadRepository.UploadPropertyImages(fileUploads, propertyId, Request);

                var res = Task.Run(() => httpContext.Post("Property/uploads/" + propertyId, uploadResponse));
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

        [Route("/enquiry")]
        [HttpPost]
        public async Task<JsonResult> CustomerEnquiry([FromBody] dynamic request)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(request));
            dynamic obj = new ExpandoObject();
            obj.Subject = Convert.ToInt32(jObject["Subject"]);
            obj.Message = Convert.ToInt32(jObject["Message"]);
            var res = Task.Run(() => httpContext.Post("Util/enquiry", obj));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
    }
}
