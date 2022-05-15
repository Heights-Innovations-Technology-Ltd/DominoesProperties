using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Models.Models;
using Repositories.Repository;


namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : Controller
    {
        public readonly IUtilRepository utilRepository;
        private readonly IStringLocalizer<UtilController> localizer;
        private ApiResponse response = new ApiResponse(false, "Error performing request, contact admin");

        public UtilController(IUtilRepository _utilRepository, IStringLocalizer<UtilController> _localizer)
        {
            utilRepository = _utilRepository;
            localizer = _localizer;
        }
    }
}
