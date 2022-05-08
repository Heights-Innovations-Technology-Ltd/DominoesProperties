using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Newtonsoft.Json;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    public class PropertyController : Controller
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly ILoggerManager logger;
        private ApiResponse response = new ApiResponse(HttpStatusCode.BadRequest, "Error performing request, contact admin");

        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
        }

        [HttpGet]
        public ApiResponse Get([FromQuery] QueryParams queryParams)
        {
            var property = propertyRepository.GetProperties(queryParams);
            var metadata = new
            {
                property.TotalCount,
                property.PageSize,
                property.CurrentPage,
                property.TotalPages,
                property.HasNext,
                property.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            logger.LogInfo($"Returned {property.TotalCount} queryParams from database.");
            response.Code = HttpStatusCode.OK;
            response.Message = "Successfull";
            response.Data = property;
            return response;
        }

        [HttpGet("{uniqueId}")]
        public ApiResponse Get(string uniqueId)
        {
            var property = propertyRepository.GetProperty(uniqueId);
            response.Code = HttpStatusCode.OK;
            response.Message = "Successfull";
            response.Data = property;
            return response;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
