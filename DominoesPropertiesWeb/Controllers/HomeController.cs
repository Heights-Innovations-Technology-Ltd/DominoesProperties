﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DominoesPropertiesWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DominoesPropertiesWeb.HttpContext;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace DominoesPropertiesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;
        private readonly ISession session;

        private readonly IHttpContext httpContext;
        private readonly IWebHostEnvironment hostEnvironment;

       
        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment _hostEnvironment, IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext _httpContext)
        {
            _logger = logger;
            _config = config;
            url = _config["Base_URL"];
            this.session = httpContextAccessor.HttpContext.Session;
            httpContext = _httpContext;
            hostEnvironment = _hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("createuser")]
        public async Task<JsonResult> Register([FromBody] Customer customer)
        {
            var res = Task.Run(() => httpContext.Post("Customer/register", customer));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("/authuser")]
        public async Task<JsonResult> Auth([FromBody] Login login)
        {
            var res = Task.Run(() => httpContext.Post("Customer/login", login));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;

            bool success = Convert.ToBoolean(data["Success"]);

            if (success)
            {
                var resObj = JsonConvert.DeserializeObject<JObject>(Convert.ToString(data["Data"]));
                this.session.SetString("Firstname", (string)resObj["firstName"]);
                this.session.SetString("Lastname", (string)resObj["lastName"]);
                this.session.SetString("ReferenceId", (string)resObj["uniqueReference"]);
                this.session.SetString("IsActive", (string)resObj["isActive"]);
                this.session.SetString("IsSubscribed", (string)resObj["isSubscribed"]);
                this.session.SetString("IsVerified", (string)resObj["isVerified"]);
                this.session.SetString("IsAccountVerified", (string)resObj["isAccountVerified"]);
                this.session.SetString("WalletId", (string)resObj["walletId"]);
                this.session.SetString("WalletBalance", (string)resObj["walletBalance"]);
                this.session.SetString("Token", (string)data["TokenObj"]);
                jsonObj.Success = success;
                jsonObj.Data = data["Message"];
            }
            else
            {
                jsonObj.Success = success;
                jsonObj.Data = data["Message"];
            }
            return Json(JsonConvert.SerializeObject(jsonObj));
        }

    }
}
