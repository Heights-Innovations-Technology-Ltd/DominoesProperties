using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repository;


namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : Controller
    {
        private readonly IUtilRepository utilRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public UtilController(IUtilRepository _utilRepository)
        {
            utilRepository = _utilRepository;
        }
    }
}
