using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "SUPER")]
    [ApiController]
    public class ConfigController : Controller
    {
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly IConfigRepository configRepository;

        public ConfigController(IConfigRepository _configRepository)
        {
            configRepository = _configRepository;
        }

        [HttpPut]
        [Route("role")]
        [Authorize(Roles = "SUPER")]
        public ApiResponse EditRole([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                response.Message = "Role name cannot be empty";
                return response;
            }
            response.Data = configRepository.EditRoles(role);
            response.Message = "Role update successful";
            return response;
        }

        [HttpPut]
        [Route("property-type")]
        [Authorize(Roles = "SUPER")]
        public ApiResponse EditPropertyType([FromBody] PropertyType property)
        {
            if (string.IsNullOrEmpty(property.Name))
            {
                response.Message = "Property type name cannot be empty";
                return response;
            }
            response.Data = configRepository.EditPropertyTypes(property);
            response.Message = "Property type update successful";
            return response;
        }

        [HttpPost]
        [Route("property-type")]
        [Authorize(Roles = "SUPER")]
        public ApiResponse AddPropertyType([FromBody] string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                response.Message = "Property type name cannot be empty";
                return response;
            }
            PropertyType pp = new()
            {
                Name = propertyName
            };
            response.Data = configRepository.AddPropertyType(pp);
            response.Message = "Property type update successful";
            return response;
        }

        [HttpDelete]
        [Route("property-type/{id}")]
        [Authorize(Roles = "SUPER")]
        public ApiResponse DeletePropertyType([FromRoute] int id)
        {
            if (id <= 0)
            {
                response.Message = "Property type id cannot be empty";
                return response;
            }
            if(!configRepository.GetPropertyTypes().Exists(x => x.Id.Equals(id)))
            {
                response.Message = $"No property type found for id {id}";
                return response;
            }
            configRepository.DeletePropertyTypes(id);
            response.Message = "Property type updated successfully";
            return response;
        }

        [HttpDelete]
        [Route("role/{id}")]
        [Authorize(Roles = "SUPER")]
        public ApiResponse DeleteRole([FromRoute] int id)
        {
            if (id <= 0)
            {
                response.Message = "Role id cannot be empty";
                return response;
            }
            if (!configRepository.GetRoles().Exists(x => x.Id.Equals(id)))
            {
                response.Message = $"No role found for id {id}";
                return response;
            }
            configRepository.DeleteRole(id);
            response.Message = "Role updated successfully";
            return response;
        }

        [HttpPost]
        [Route("role")]
        [Authorize(Roles = "SUPER")]
        public ApiResponse AddRole([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                response.Message = "Role name cannot be empty";
                return response;
            }
            Role pp = new()
            {
                CreatedBy = HttpContext.User.Identity.Name,
                RoleName =  role.RoleName
            };
            response.Data = configRepository.AddRole(pp);
            response.Message = "Role created successfully";
            return response;
        }
    }
}