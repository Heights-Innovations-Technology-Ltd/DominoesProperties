using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IUtilRepository utilRepository;
        private readonly ApiResponse response;
        private readonly IConfiguration configuration;
        private readonly IUploadRepository uploadRepository;
        private readonly IAdminRepository adminRepository;


        public PropertyController(IPropertyRepository _propertyRepository, ILoggerManager _logger,
            IUtilRepository _utilRepository, IConfiguration _configuration, IUploadRepository _uploadRepository, IAdminRepository _adminRepository)
        {
            propertyRepository = _propertyRepository;
            logger = _logger;
            utilRepository = _utilRepository;
            configuration = _configuration;
            uploadRepository = _uploadRepository;
            adminRepository = _adminRepository;
            response = new(false, "Error performing request, contact admin");
        }

        [HttpGet]
        public ApiResponse Property([FromQuery] QueryParams queryParams, [FromQuery] PropertyFilter propertyFilter)
        {
            List<Properties> properties = new();

            if (propertyFilter != null)
            {
                var prop = propertyRepository.GetProperties();

                if (!string.IsNullOrEmpty(propertyFilter.Category))
                    prop = prop.Where(x => x.Status == propertyFilter.Category).ToList();
                if (propertyFilter.MinPrice > 0)
                    prop = prop.Where(x => x.UnitPrice >= propertyFilter.MinPrice).ToList();
                if (propertyFilter.MaxPrice > 0)
                    prop = prop.Where(x => x.UnitPrice >= propertyFilter.MaxPrice).ToList();
                if (!string.IsNullOrEmpty(propertyFilter.Location))
                    prop = prop.Where(x => x.Location.Contains(propertyFilter.Location)).ToList();

                prop.ForEach(x =>
                {
                    var prop2 = ClassConverter.EntityToProperty(x);
                    prop2.Description = ClassConverter.ConvertDescription(propertyRepository.GetDescriptionByPropertyId(prop2.UniqueId));
                    properties.Add(prop2);

                    //var uploaded = uploadRepository.GetUploads(x.Id).Where(x => x.UploadType.Equals(UploadType.PICTURE.ToString())).Select(x => x.Url).ToList();
                    //prop2.Data = uploaded;
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

                (int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata2 =
                (
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
        }

        [HttpGet("{uniqueId}")]
        public ApiResponse Property(string uniqueId)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            Properties properties = ClassConverter.EntityToProperty(property);
            properties.Description = ClassConverter.ConvertDescription(propertyRepository.GetDescriptionByPropertyId(property.UniqueId));
            Dictionary<string, object> Uploads = new();
            var uploaded = uploadRepository.GetUploads(property.Id);
            Uploads.Add("Images", uploaded.Where(x => x.UploadType.Equals(UploadType.PICTURE.ToString())).Select(x => x.Url).ToList());
            Uploads.Add("Document", uploaded.Where(x => x.UploadType.Equals(UploadType.DOCUMENT.ToString())).Select(x => x.Url).ToList());
            Uploads.Add("Cover", uploaded.Where(x => x.UploadType.Equals(UploadType.COVER.ToString())).Select(x => x.Url).ToList());
            properties.Data = Uploads;
            response.Success = true;
            response.Message = "Successfull";
            response.Data = properties;
            return response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, SUPER")]
        public ApiResponse Property([FromBody] Properties properties)
        {
            if (properties.AllowSharing && properties.MinimumSharingPercentage < 10)
            {
                response.Message = $"Sharing percentage cannot be less and 10% for property that allows sharing";
                return response;
            }

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
        [Authorize(Roles = "ADMIN")]
        public ApiResponse Property(string uniqueId, [FromBody] UpdateProperty updateProperty)
        {
            Property property = propertyRepository.GetProperty(uniqueId);
            if (property == null)
            {
                response.Message = "Username not found, kindly check and try again";
                return response;
            }
            property.Location = string.IsNullOrEmpty(updateProperty.Location) ? property.Location : updateProperty.Location;
            property.Type = updateProperty.Type == null ? property.Type : updateProperty.Type.Value;
            property.TotalUnits = updateProperty.TotalUnits > 0 ? updateProperty.TotalUnits : property.TotalUnits;
            property.UnitPrice = updateProperty.UnitPrice > 0 ? updateProperty.UnitPrice : property.UnitPrice;
            property.ClosingDate = updateProperty.ClosingDate == null ? property.ClosingDate : updateProperty.ClosingDate;
            property.TargetYield = updateProperty.TargetYield.Value > 0 ? updateProperty.TargetYield.Value : property.TargetYield;
            property.ProjectedGrowth = updateProperty.ProjectedGrowth.Value > 0 ? updateProperty.ProjectedGrowth.Value : property.ProjectedGrowth;
            property.InterestRate = updateProperty.InterestRate.Value > 0 ? updateProperty.InterestRate.Value : property.InterestRate;
            property.Longitude = string.IsNullOrEmpty(updateProperty.Longitude) ? property.Longitude : updateProperty.Longitude;
            property.Latitude = string.IsNullOrEmpty(updateProperty.Latitude) ? property.Latitude : updateProperty.Latitude;
            property.Summary = string.IsNullOrEmpty(updateProperty.Summary) ? property.Summary : updateProperty.Summary;
            property.VideoLink = string.IsNullOrEmpty(updateProperty.VideoLink) ? property.VideoLink : updateProperty.VideoLink;
            property.AllowSharing = updateProperty.AllowSharing;
            property.MinimumSharingPercentage = updateProperty.MinimumSharingPercentage > 0 ? updateProperty.MinimumSharingPercentage : property.MinimumSharingPercentage;

            response.Data = propertyRepository.UpdateProperty(property);
            response.Success = true;
            response.Message = "Successful";
            return response;
        }

        [HttpDelete("{uniqueId}")]
        [Authorize(Roles = "SUPER")]
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
        [Authorize("ADMIN, SUPER")]
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

        //[HttpGet("feature")]
        //public ApiResponse GetFeatureProperty()
        //{
        //    response.Data = propertyRepository.GetProperties().Where(x => x.Status.Equals(PropertyStatus.OPEN_FOR_INVESTMENT) || x.Status.Equals(PropertyStatus.ONGOING_CONSTRUCTION));
        //    response.Success = true;
        //    response.Message = "Property types fetched successfully!";
        //    return response;
        //}

        [HttpPost("uploads/{propertyId}")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse UploadFile(string propertyId, [FromBody][Required(ErrorMessage = "No upload found")][MinLength(1, ErrorMessage = "Upload atleast 1 file")] List<PropertyFileUpload> passport)
        {
            try
            {
                List<Propertyupload> propertyUploads = new();
                var property = propertyRepository.GetProperty(propertyId);
                if (property == null)
                {
                    response.Message = "Invalid property selected";
                    return response;
                }

                bool isError = false;
                passport.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.ImageName) || !string.IsNullOrEmpty(x.Url))
                    {
                        propertyUploads.Add(new Propertyupload
                        {
                            DateUploaded = x.DateUploaded,
                            ImageName = x.ImageName,
                            PropertyId = property.Id,
                            UploadType = x.UploadType.ToString(),
                            Url = x.Url,
                            AdminEmail = HttpContext.User.Identity.Name
                        });
                    }
                    else
                    {
                        response.Message = "One or more upload data is incorrect, kindly check and try aagin";
                        isError = true;
                    }
                });

                if (isError)
                {
                    return response;
                }
                if (uploadRepository.NewUpload(propertyUploads))
                {
                    response.Success = true;
                    response.Message = "Property images successfully uploaded";
                    return response;
                }
                else
                {
                    response.Message = $"Error uploading property image and document";
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}