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
        private ApiResponse response = new ApiResponse(HttpStatusCode.BadRequest, "Error performing request, contact admin");

        public UtilController(IUtilRepository _utilRepository, IStringLocalizer<UtilController> _localizer)
        {
            utilRepository = _utilRepository;
            localizer = _localizer;
        }

        [HttpGet("property-types")]
        public ApiResponse GetPropertyTypes()
        {
            response.Data = utilRepository.GetPropertyTypes();
            response.Code = HttpStatusCode.OK;
            response.Message = localizer["Response.Success"];
            return response;
        }
    }
}
