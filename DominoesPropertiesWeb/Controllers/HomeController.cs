using System;
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
            if (!ModelState.IsValid)
            {
                jsonObj.success = false;
                var m = ModelState;
                //jsonObj.Data = ModelState;
                return jsonObj;
            }

            var res = Task.Run(() => httpContext.Post("User/register", customer));
            var data = await res.GetAwaiter().GetResult();
            return Json(JsonConvert.SerializeObject(data));
        }
    }
}
