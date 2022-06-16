using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        private readonly IAdminRepository adminRepository;


        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger, IStringLocalizer<PropertyController> _localizer,
            IUtilRepository _utilRepository, IConfiguration _configuration, IUploadRepository _uploadRepository, IAdminRepository _adminRepository)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
            localizer = _localizer;
            utilRepository = _utilRepository;
            configuration = _configuration;
            uploadRepository = _uploadRepository;
            adminRepository = _adminRepository;
        }

        [HttpGet]
        public ApiResponse Property([FromQuery] QueryParams queryParams, [FromQuery] PropertyFilter propertyFilter)
        {

            List<Properties> properties = new();

            if (propertyFilter != null)
            {
                var prop = propertyRepository.GetProperties();

                if (propertyFilter.Category != null)
                    prop = prop.Where(x => x.Status == propertyFilter.Category).ToList();
                if (propertyFilter.MinPrice > 0)
                    prop = prop.Where(x => x.UnitPrice >= propertyFilter.MinPrice).ToList();
                if (propertyFilter.MaxPrice > 0)
                    prop = prop.Where(x => x.UnitPrice >= propertyFilter.MaxPrice).ToList();

                prop.ForEach(x =>
                {
                    var prop2 = ClassConverter.EntityToProperty(x);
                    prop2.Description = ClassConverter.ConvertDescription(propertyRepository.GetDescriptionByPropertyId(prop2.UniqueId));
                    properties.Add(prop2);
                });

                if (propertyFilter.AirConditioned != null)
                    properties = properties.Where(x => x.Description.AirConditioned == propertyFilter.AirConditioned).ToList();
                if (propertyFilter.Basement != null)
                    properties = properties.Where(x => x.Description.Basement == propertyFilter.Basement).ToList();
                if (propertyFilter.Bathroom != null)
                    properties = properties.Where(x => x.Description.Bathroom == propertyFilter.Bathroom).ToList();
                if (propertyFilter.Bedroom != null)
                    properties = properties.Where(x => x.Description.Bedroom == propertyFilter.Bedroom).ToList();
                if (propertyFilter.Fireplace != null)
                    properties = properties.Where(x => x.Description.Fireplace == propertyFilter.Fireplace).ToList();
                if (propertyFilter.Floor != null)
                    properties = properties.Where(x => x.Description.FloorLevel == propertyFilter.Floor).ToList();
                if (propertyFilter.Gym != null)
                    properties = properties.Where(x => x.Description.Gym == propertyFilter.Gym).ToList();
                if (propertyFilter.Laundry != null)
                    properties = properties.Where(x => x.Description.Laundry == propertyFilter.Laundry).ToList();
                if (propertyFilter.Parking != null)
                    properties = properties.Where(x => x.Description.Parking == propertyFilter.Parking).ToList();
                if (propertyFilter.Refridgerator != null)
                    properties = properties.Where(x => x.Description.Refrigerator == propertyFilter.Refridgerator).ToList();
                if (propertyFilter.SecurityGuard != null)
                    properties = properties.Where(x => x.Description.SecurityGuard == propertyFilter.SecurityGuard).ToList();
                if (propertyFilter.SwimmingPool != null)
                    properties = properties.Where(x => x.Description.SwimmingPool == propertyFilter.SwimmingPool).ToList();
                if (propertyFilter.Toilet != null)
                    properties = properties.Where(x => x.Description.Toilet == propertyFilter.Toilet).ToList();

                PagedList<Properties> propList = PagedList<Properties>.ToPagedList(properties.OrderBy(on => on.DateCreated).AsQueryable(),
                queryParams.PageNumber, queryParams.PageSize);

                (int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata2 = (
                propList.TotalCount,
                propList.PageSize,
                propList.CurrentPage,
                propList.TotalPages,
                propList.HasNext,
                propList.HasPrevious
            );

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata2));
                logger.LogInfo($"Returned {propList.TotalCount} queryParams from database.");
                response.Success = propList.Count > 0;
                response.Message = response.Success ? "Successfull" : "No property found";
                response.Data = propList;
                return response;
            }
            return response;

            //PagedList<Property> property = propertyRepository.GetProperties(queryParams);
            //property.ForEach(x =>
            //{
            //    var prop = ClassConverter.EntityToProperty(x);
            //    prop.Description = ClassConverter.ConvertDescription(propertyRepository.GetDescriptionByPropertyId(prop.UniqueId));
            //    properties.Add(prop);
            //});

            //(int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata = (
            //    property.TotalCount,
            //    property.PageSize,
            //    property.CurrentPage,
            //    property.TotalPages,
            //    property.HasNext,
            //    property.HasPrevious
            //);
            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            //logger.LogInfo($"Returned {property.TotalCount} queryParams from database.");
            //response.Success = properties.Count > 0;
            //response.Message = response.Success ? "Successfull" : "No property found";
            //response.Data = properties;
            //return response;
        }

        [HttpGet("{uniqueId}")]
        public ApiResponse Property(string uniqueId)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            Properties properties = ClassConverter.EntityToProperty(property);
            properties.Description = ClassConverter.ConvertDescription(propertyRepository.GetDescriptionByPropertyId(property.UniqueId));
            response.Success = true;
            response.Message = "Successfull";
            response.Data = properties;
            return response;
        }

        [HttpPost]
        [Authorize]
        public ApiResponse Property([FromBody] Properties properties)
        {
            Property property = ClassConverter.PropertyToEntity(properties);
            property.CreatedBy = adminRepository.GetUser(HttpContext.User.Identity.Name).Email;
            Description description = ClassConverter.DescriptionToEntity(properties.Description);
            description.PropertyId = propertyRepository.AddNewProperty(property).UniqueId;
            propertyRepository.AddPropertyDescription(description);
            response.Success = true;
            response.Message = $"Property {property.Name} created successfully";
            return response;
        }

        [HttpPut("{uniqueId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Property(string uniqueId, [FromBody] UpdateProperty updateProperty)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            if (property == null)
            {
                response.Message = "Username name not found, kindly check and try again";
                return response;
            }
            property.Location = string.IsNullOrEmpty(updateProperty.Location) ? property.Location : updateProperty.Location;
            property.Type = updateProperty.Type == null ? property.Type : updateProperty.Type;
            property.TotalUnits = updateProperty.TotalUnits > 0 ? updateProperty.TotalUnits : property.TotalUnits;
            property.UnitPrice = updateProperty.UnitPrice > 0 ? updateProperty.UnitPrice : property.UnitPrice;
            property.ClosingDate = updateProperty.ClosingDate == null ? property.ClosingDate : updateProperty.ClosingDate;
            property.TargetYield = updateProperty.TargetYield > 0 ? updateProperty.TargetYield : property.TargetYield;
            property.ProjectedGrowth = updateProperty.ProjectedGrowth > 0 ? updateProperty.ProjectedGrowth : property.ProjectedGrowth;
            property.InterestRate = updateProperty.InterestRate > 0 ? updateProperty.InterestRate : property.InterestRate;
            property.Longitude = string.IsNullOrEmpty(updateProperty.Longitude) ? property.Longitude : updateProperty.Longitude;
            property.Latitude = string.IsNullOrEmpty(updateProperty.Latitude) ? property.Latitude : updateProperty.Latitude;

            response.Data = propertyRepository.UpdateProperty(property);
            response.Success = true;
            response.Message = "Successful";
            return response;
        }

        [HttpDelete("{uniqueId}")]
        [Authorize(Roles = "Admin")]
        public ApiResponse Delete(string uniqueId)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            if (property == null)
            {
                response.Message = $"No property with the given name {uniqueId} found";
                return response;
            }
            property.IsDeleted = true;
            response.Success = true;
            response.Message = "Successful";
            return response;
        }

        [HttpPut("description/{propertyId}")]
        [Authorize]
        public ApiResponse UpdateDescription(string propertyId, [FromBody] PropertyDescription description)
        {
            var propDescription = propertyRepository.GetDescriptionByPropertyId(propertyId);
            if (propDescription != null)
            {
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
                response.Message = "Successful";
                return response;
            }
            else
            {
                response.Message = $"No property with the given name {propertyId} found";
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
        public async Task<ApiResponse> UploadPassportAsync(long propertyId, [Required(ErrorMessage = "No upload found")][MinLength(1, ErrorMessage = "Upload atleast 1 file")] List<IFormFile> passport)
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
                    ImageName = (propertyId + count++).ToString(),
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