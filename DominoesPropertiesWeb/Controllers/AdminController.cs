using Microsoft.AspNetCore.Mvc;

namespace DominoesPropertiesWeb.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
