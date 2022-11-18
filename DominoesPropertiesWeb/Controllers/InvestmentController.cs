﻿using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Repository;
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
        private readonly IUploadsRepository _uploadRepository;
        private readonly IHttpContext httpContext;

        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public InvestmentController(IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext httpContext, IUploadsRepository uploadRepository)
        {
            _config = config;
            this.session = httpContextAccessor.HttpContext.Session;
            this.httpContext = httpContext;
            url = _config["Base_URL"];
            _uploadRepository = uploadRepository;
        }

        public IActionResult Index()
        {
            var userAuth = this.session.GetString("Token");
            if (userAuth == null || userAuth.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }

        [HttpGet("/Investment/viewinvestment/{uniqueid}")]
        public IActionResult ViewInvestment()
        {
            var userAuth = this.session.GetString("Token");
            if (userAuth == null || userAuth.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }

        [Route("/invest")]
        public async Task<JsonResult> propertyInvestment([FromBody] dynamic property)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(property));

            dynamic obj = new ExpandoObject();

            obj.PropertyUniqueId = Convert.ToString(jObject["propertyUniqueId"]);
            obj.Units = Convert.ToInt16(jObject["units"]);
            obj.PaymentChannel = Convert.ToInt32(jObject["channel"]);
            obj.IsSharing = 0;
            var res = Task.Run(() => httpContext.Post("Investment", obj));
            var data = await res.GetAwaiter().GetResult();
            return Json(JsonConvert.SerializeObject(data));
        }
        
        [Route("/get-investments/{customerId}")]
        public async Task<JsonResult> GetCustomerInvestments(string customerId)
        {
            var res = Task.Run(() => httpContext.Get($"Investment/{customerId}"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("/get-pending-investments/{customerId}")]
        public async Task<JsonResult> GetPendingInvestments(string customerId)
        {
            var res = Task.Run(() => httpContext.Get($"Investment/offline/{customerId}"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
        
        [Route("/get-pending-investments")]
        public async Task<JsonResult> GetAllPendingInvestments()
        {
            var res = Task.Run(() => httpContext.Get($"Investment"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("/get-investment/{propetyUniqueId}")]
        public async Task<JsonResult> GetInvestmentById(string propetyUniqueId)
        {
            var res = Task.Run(() => httpContext.Get($"Investment/property/{propetyUniqueId}"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [HttpPost("/upload-proof/{paymentRef}")]
        public async Task<JsonResult> UploadDoc(string paymentRef)
        {
            var json = Request.Form.Files;

            if (json.Count > 0)
            {
                IFormFile file = json[0];
                var uploadResponse = await _uploadRepository.UploadCustomerPassportAsync(file, paymentRef, Request);
                var res = Task.Run(() => httpContext.Put($"Investment/proof-of-payment/{paymentRef}", uploadResponse));
                //var data = await res.GetAwaiter().GetResult();
                await Task.WhenAll(res);
                var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
                return Json(JsonConvert.SerializeObject(data));
            }
            return Json(this.StatusCode(StatusCodes.Status204NoContent, "Empty request body"));
        }
    }
}
