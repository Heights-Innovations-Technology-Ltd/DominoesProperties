using Microsoft.AspNetCore.Mvc;

namespace DominoesPropertiesWeb.Controllers
{
    public class InvestmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
