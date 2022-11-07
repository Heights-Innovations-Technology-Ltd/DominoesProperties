using System.Threading.Tasks;
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
        private readonly IConfigRepository _configRepository;
        private readonly ApiResponse _response = new(false, "Error performing request, contact admin");

        public ConfigController(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        [HttpPut]
        [Route("role")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> EditRole([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                _response.Message = "Role name cannot be empty";
                return _response;
            }

            _response.Data = await _configRepository.EditRoles(role);
            _response.Success = true;
            _response.Message = "Role update successful";
            return _response;
        }

        [HttpPut]
        [Route("property-type")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> EditPropertyType([FromBody] PropertyType property)
        {
            if (string.IsNullOrEmpty(property.Name))
            {
                _response.Message = "Property type name cannot be empty";
                return _response;
            }

            _response.Data = await _configRepository.EditPropertyTypes(property);
            _response.Success = true;
            _response.Message = "Property type updated successful";
            return _response;
        }

        [HttpPost]
        [Route("property-type")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> AddPropertyType([FromBody] string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                _response.Message = "Property type name cannot be empty";
                return _response;
            }

            PropertyType pp = new()
            {
                Name = propertyName
            };
            _response.Data = await _configRepository.AddPropertyType(pp);
            _response.Success = true;
            _response.Message = "Property type created successful";
            return _response;
        }

        [HttpDelete]
        [Route("property-type/{id}")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> DeletePropertyType([FromRoute] int id)
        {
            if (id <= 0)
            {
                _response.Message = "Property type id cannot be empty";
                return _response;
            }

            if (!_configRepository.GetPropertyTypes().Exists(x => x.Id.Equals(id)))
            {
                _response.Message = $"No property type found for id {id}";
                return _response;
            }

            await _configRepository.DeletePropertyTypes(id);
            _response.Success = true;
            _response.Message = "Property type deleted successfully";
            return _response;
        }

        [HttpDelete]
        [Route("role/{id:int}")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> DeleteRole([FromRoute] int id)
        {
            if (id <= 0)
            {
                _response.Message = "Role id cannot be empty";
                return _response;
            }

            if (!_configRepository.GetRoles().Exists(x => x.Id.Equals(id)))
            {
                _response.Message = $"No role found for id {id}";
                return _response;
            }

            await _configRepository.DeleteRole(id);
            _response.Success = true;
            _response.Message = "Role deleted successfully";
            return _response;
        }

        [HttpPost]
        [Route("role")]
        [Authorize(Roles = "SUPER")]
        public async Task<ApiResponse> AddRole([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                _response.Message = "Role name cannot be empty";
                return _response;
            }

            Role pp = new()
            {
                CreatedBy = HttpContext.User.Identity.Name,
                RoleName = role.RoleName
            };
            _response.Data = await _configRepository.AddRole(pp);
            _response.Success = true;
            _response.Message = "Role created successfully";
            return _response;
        }
    }
}