using DominoesPropertiesWeb.HttpContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISession session;
        private readonly IConfiguration _config;

        private readonly IHttpContext httpContext;
        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public DashboardController(IHttpContextAccessor httpContextAccessor, IConfiguration config, IHttpContext httpContext)
        {
            this.session = httpContextAccessor.HttpContext.Session;
            _config = config;
            url = _config["Base_URL"];
            this.httpContext = httpContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }

        [Route("get-customer")]
        public async Task<JsonResult> Customer()
        {
            var res = Task.Run(() => httpContext.Get("Customer"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            this.session.Clear();
            return Json(true);
        }
    }
}
