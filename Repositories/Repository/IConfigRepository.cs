using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Models;

namespace Repositories.Repository
{
    public interface IConfigRepository
    {
        Task<Role> AddRole(Role role);
        Task<PropertyType> AddPropertyType(PropertyType propertyType);
        List<Role> GetRoles();
        List<PropertyType> GetPropertyTypes();
        Task<bool> DeleteRole(int RoleId);
        Task<bool> DeletePropertyTypes(int propertyType);
        Task<List<Role>> EditRoles(Role role);
        Task<List<PropertyType>> EditPropertyTypes(PropertyType property);
    }
}

