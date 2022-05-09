using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DominoesProperties.Helper;
using DominoesProperties.Localize;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    public class PropertyController : Controller
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly ILoggerManager logger;
        private readonly IStringLocalizer<Resource> localizer;
        private readonly ICustomerRepository customerRepository;
        private ApiResponse response = new ApiResponse(HttpStatusCode.BadRequest, "Error performing request, contact admin");

        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger, IStringLocalizer<Resource> _localizer, ICustomerRepository _customerRepository)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
            localizer = _localizer;
            customerRepository = _customerRepository;
        }

        [HttpGet]
        public ApiResponse Property([FromQuery] QueryParams queryParams)
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
        public ApiResponse Property(string uniqueId)
        {
            var property = propertyRepository.GetProperty(uniqueId);
            response.Code = HttpStatusCode.OK;
            response.Message = "Successfull";
            response.Data = property;
            return response;
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public ApiResponse Property([FromBody] Properties properties, [Required(ErrorMessage ="Only admin users can create property")] [FromHeader] string admin)
        {
            var property = ClassConverter.PropertyToEntity(properties);
            propertyRepository.AddNewProperty(property);
            response.Code = HttpStatusCode.OK;
            response.Message = localizer["201"].ToString().Replace("{params}", $"Property {property.Name}");
            return response;
        }

        [HttpPut("{uniqueId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Property(string uniqueId, [FromBody] UpdateProperty updateProperty)
        {
            var property = propertyRepository.GetProperty(uniqueId);
            if(property == null){
                response.Message = localizer["Username.Error"];
                return response;
            }
            property.Location = string.IsNullOrEmpty(updateProperty.Location) ? property.Location : updateProperty.Location;
            property.Type = updateProperty.Type == null ? property.Type : updateProperty.Type;
            property.TotalUnits = updateProperty.TotalUnits > 0 ? updateProperty.TotalUnits : property.TotalUnits;
            property.UnitPrice = updateProperty.UnitPrice > 0 ? updateProperty.UnitPrice : property.UnitPrice;
            property.ClosingDate = updateProperty.ClosingDate == null ? property.ClosingDate : updateProperty.ClosingDate;
            property.TargetYield = updateProperty.TargetYield > 0 ? updateProperty.TargetYield : property.TargetYield;
            property.ProjectedGrowth = updateProperty.ProjectedGrowth > 0 ? updateProperty.ProjectedGrowth : property.ProjectedGrowth;
            property.InterestRate = updateProperty.InterestRate> 0 ? updateProperty.InterestRate : property.InterestRate;
            property.Longitude = string.IsNullOrEmpty(updateProperty.Longitude) ? property.Longitude : updateProperty.Longitude;
            property.Latitude = string.IsNullOrEmpty(updateProperty.Latitude) ? property.Latitude : updateProperty.Latitude;

            response.Data = propertyRepository.UpdateProperty(property);
            response.Code = HttpStatusCode.OK;
            response.Message = localizer["200"];
            return response;
        }

        [HttpDelete("{uniqueId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Delete(string uniqueId)
        {
            var property = propertyRepository.GetProperty(uniqueId);
            if(property == null){
                response.Message = localizer["Property.Id.Error"];
                return response;
            }
            property.IsDeleted = true;
            response.Code = HttpStatusCode.OK;
            response.Message = localizer["Property.Id.Error"];
            return response;
        }

        [HttpPut("propertyId")]
        [Authorize]
        public ApiResponse UpdateDescription(string propertyId, [FromBody] PropertyDescription description){
            var propDescription = propertyRepository.GetProperty(propertyId).DescriptionNavigation;
            if(propDescription != null){
                propDescription.AirConditioned = description.AirConditioned;
                propDescription.Basement = description.Basement;
                propDescription.Bathroom = description.Bathroom;
                propDescription.Bedroom = description.Bedroom;
                propDescription.Fireplace = description.Fireplace;
                propDescription.FloorLevel = description.FloorLevel;
                propDescription.Gym = description.Gym;
                propDescription.LandSize = description.LandSize;
                propDescription.Laundry = description.Laundry;
                propDescription.Parking = description.Parking;
                propDescription.Refrigerator = description.Refrigerator;
                propDescription.SecurityGuard = description.SecurityGuard;
                propDescription.SwimmingPool = description.SwimmingPool;
                propDescription.Toilet = description.Toilet;

                response.Data = propertyRepository.UpdatePropertyDescription(propDescription);
                response.Code = HttpStatusCode.OK;
                response.Message = localizer["200"];
                return response;
            }
            else{
                response.Message = localizer["Property.Id.Error"];
                return response;
            }
        }
    }
}
