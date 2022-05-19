using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DominoesPropertiesWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISession session;
        public DashboardController(IHttpContextAccessor httpContextAccessor)
        {
            this.session = httpContextAccessor.HttpContext.Session;
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

        [Route("/logout")]
        public IActionResult Logout()
        {
            this.session.Clear();
            return Json(true);
        }
    }
}
