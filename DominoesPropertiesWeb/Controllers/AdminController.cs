using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ISession session;
        private readonly IHttpContext httpContext;
        private readonly IWebHostEnvironment hostEnvironment;
        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public AdminController( IWebHostEnvironment _hostEnvironment, IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext _httpContext)
        {
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

        public IActionResult Create()
        {
            return View();
        }

        [Route("/authadmin")]
        public async Task<JsonResult> Auth([FromBody] Login login)
        {
            var res = Task.Run(() => httpContext.Post("Admin/login", login));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;

            bool success = Convert.ToBoolean(data["success"]);

            if (success)
            {
                var resObj = JsonConvert.DeserializeObject<JObject>(Convert.ToString(data["data"]));
                this.session.SetString("Email", (string)resObj["email"]);
                this.session.SetString("RoleFK", (string)resObj["roleFk"]);
                this.session.SetString("IsActive", (string)resObj["isActive"]);
                this.session.SetString("Token", (string)data["TokenObj"]);
                jsonObj.success = success;
                jsonObj.data = data["message"];
            }
            else
            {
                jsonObj.success = success;
                jsonObj.data = data["data"];
            }
            return Json(JsonConvert.SerializeObject(jsonObj));
        }
    }
}
