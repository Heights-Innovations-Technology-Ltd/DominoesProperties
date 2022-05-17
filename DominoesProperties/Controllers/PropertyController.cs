using System.Reflection;
using System.Resources;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<PropertyController> localizer;
        private readonly IUtilRepository utilRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly ResourceManager rm = new("item", Assembly.GetExecutingAssembly());


        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger, IStringLocalizer<PropertyController> _localizer,
            IUtilRepository _utilRepository)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
            localizer = _localizer;
            utilRepository = _utilRepository;
        }

        [HttpGet]
        public ApiResponse Property([FromQuery] QueryParams queryParams)
        {
            PagedList<Property> property = propertyRepository.GetProperties(queryParams);
            (int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata = (
                property.TotalCount,
                property.PageSize,
                property.CurrentPage,
                property.TotalPages,
                property.HasNext,
                property.HasPrevious
            );
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            logger.LogInfo($"Returned {property.TotalCount} queryParams from database.");
            response.Success = true;
            response.Message = "Successfull";
            response.Data = property;
            return response;
        }

        [HttpGet("{uniqueId}")]
        public ApiResponse Property(string uniqueId)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            response.Success = true;
            response.Message = "Successfull";
            response.Data = property;
            return response;
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public ApiResponse Property([FromBody] Properties properties)
        {
            Property property = ClassConverter.PropertyToEntity(properties);
            _ = propertyRepository.AddNewProperty(property);
            response.Success = true;
            response.Message = localizer["Response.Created"].ToString().Replace("{params}", $"Property {property.Name}");
            return response;
        }

        [HttpPut("{uniqueId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Property(string uniqueId, [FromBody] UpdateProperty updateProperty)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
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
            response.Success = true;
            response.Message = localizer["Response.Success"];
            return response;
        }

        [HttpDelete("{uniqueId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Delete(string uniqueId)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            if(property == null){
                response.Message = localizer["Property.Id.Error"];
                return response;
            }
            property.IsDeleted = true;
            response.Success = true;
            response.Message = localizer["Property.Id.Error"];
            return response;
        }

        [HttpPut("description/{propertyId}")]
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
                response.Success = true;
                response.Message = localizer["Response.Success"];
                return response;
            }
            else{
                response.Message = localizer["Property.Id.Error"];
                return response;
            }
        }

        [HttpGet("types")]
        public ApiResponse GetPropertyTypes()
        {
            response.Data = utilRepository.GetPropertyTypes();
            response.Success = true;
            response.Message = rm.GetString("welcome"); //localizer["Response.Success"];
            return response;
        }
    }
}
