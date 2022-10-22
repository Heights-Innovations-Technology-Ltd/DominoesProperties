using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositories.Repository;
using System.Threading.Tasks;

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
        public async Task<ApiResponse> EditRole([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                response.Message = "Role name cannot be empty";
                return response;
            }
            response.Data = await configRepository.EditRoles(role);
            response.Success = true;
            response.Message = "Role update successful";
            return response;
        }

        [HttpPut]
        [Route("property-type")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> EditPropertyType([FromBody] PropertyType property)
        {
            if (string.IsNullOrEmpty(property.Name))
            {
                response.Message = "Property type name cannot be empty";
                return response;
            }
           
            response.Data = await configRepository.EditPropertyTypes(property);
            response.Success = true;
            response.Message = "Property type updated successful";
            return response;
        }

        [HttpPost]
        [Route("property-type")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> AddPropertyType([FromBody] string propertyName)
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
            response.Data = await configRepository.AddPropertyType(pp);
            response.Success = true;
            response.Message = "Property type created successful";
            return response;
        }

        [HttpDelete]
        [Route("property-type/{id}")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> DeletePropertyType([FromRoute] int id)
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
            await configRepository.DeletePropertyTypes(id);
            response.Success = true;
            response.Message = "Property type deleted successfully";
            return response;
        }

        [HttpDelete]
        [Route("role/{id}")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> DeleteRole([FromRoute] int id)
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
            await configRepository.DeleteRole(id);
            response.Success = true;
            response.Message = "Role deleted successfully";
            return response;
        }

        [HttpPost]
        [Route("role")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> AddRole([FromBody] Role role)
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
            response.Data =  await configRepository.AddRole(pp);
            response.Success = true;
            response.Message = "Role created successfully";
            return response;
        }
    }
}