using Microsoft.AspNetCore.Mvc;

namespace DominoesPropertiesWeb.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
