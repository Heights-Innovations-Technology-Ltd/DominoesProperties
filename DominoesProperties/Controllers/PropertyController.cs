using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration configuration;
        private readonly IUploadRepository uploadRepository;


        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger, IStringLocalizer<PropertyController> _localizer,
            IUtilRepository _utilRepository, IConfiguration _configuration, IUploadRepository _uploadRepository)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
            localizer = _localizer;
            utilRepository = _utilRepository;
            configuration = _configuration;
            uploadRepository = _uploadRepository;
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
        [Authorize(Roles = "Admin")]
        public ApiResponse Property([FromBody] Properties properties)
        {
            Property property = ClassConverter.PropertyToEntity(properties);
            property.Description = propertyRepository.AddPropertyDescription(property.Description1).Id;
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
            response.Message = "Property types fetched successfully!";
            return response;
        }

        [HttpPost("uploads/{propertyId}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ApiResponse> UploadPassportAsync(string propertyId, [Required(ErrorMessage = "No upload found")][MinLength(1, ErrorMessage = "Upload atleast 1 file")] List<IFormFile> passport)
        {
            var container = new BlobContainerClient(configuration["BlobClient:Url"], "properties");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            int count = 0;
            PropertyUpload[] properties = Array.Empty<PropertyUpload>();
            passport.ForEach(async x =>
            {
                var blob = container.GetBlobClient($"{propertyId + count++}.{x.FileName[x.FileName.LastIndexOf(".")..]}");
                using (var fileStream = x.OpenReadStream())
                {
                    _ = await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = x.ContentType });
                }

                properties[count] = new PropertyUpload
                {
                    DateUploaded = DateTime.Now,
                    ImageName = propertyId + count++,
                    PropertyId = propertyId,
                    Url = blob.Uri.ToString()
                };
            });

            if (uploadRepository.NewUpload(properties))
            {
                response.Success = true;
                response.Message = "Passport successfully uploaded";
            }
            response.Message = "Error uploading property images";
            return response;
        }
    }
}
=======
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration configuration;
        private readonly IUploadRepository uploadRepository;


        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger, IStringLocalizer<PropertyController> _localizer,
            IUtilRepository _utilRepository, IConfiguration _configuration, IUploadRepository _uploadRepository)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
            localizer = _localizer;
            utilRepository = _utilRepository;
            configuration = _configuration;
            uploadRepository = _uploadRepository;
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
        [Authorize(Roles = "Admin")]
        public ApiResponse Property([FromBody] Properties properties)
        {
            Property property = ClassConverter.PropertyToEntity(properties);
            property.Description = propertyRepository.AddPropertyDescription(property.Description1).Id;
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
            response.Message = "Property types fetched successfully!";
            return response;
        }

        [HttpPost("uploads/{propertyId}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ApiResponse> UploadPassportAsync(string propertyId, [Required(ErrorMessage = "No upload found")][MinLength(1, ErrorMessage = "Upload atleast 1 file")] List<IFormFile> passport)
        {
            var container = new BlobContainerClient(configuration["BlobClient:Url"], "properties");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);

            int count = 0;
            PropertyUpload[] properties = Array.Empty<PropertyUpload>();
            passport.ForEach(async x =>
            {
                var blob = container.GetBlobClient($"{propertyId + count++}.{x.FileName[x.FileName.LastIndexOf(".")..]}");
                using (var fileStream = x.OpenReadStream())
                {
                    _ = await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = x.ContentType });
                }

                properties[count] = new PropertyUpload
                {
                    DateUploaded = DateTime.Now,
                    ImageName = propertyId + count++,
                    PropertyId = propertyId,
                    Url = blob.Uri.ToString()
                };
            });

            if (uploadRepository.NewUpload(properties))
            {
                response.Success = true;
                response.Message = "Passport successfully uploaded";
            }
            response.Message = "Error uploading property images";
            return response;
        }
    }
}
>>>>>>> 6a3a52ef0c9caff7c1feaf873d2c262ff7710217
