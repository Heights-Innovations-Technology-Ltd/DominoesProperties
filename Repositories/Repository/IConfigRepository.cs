using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IConfigRepository
    {
        Role AddRole(Role role);
        PropertyType AddPropertyType(PropertyType propertyType);
        List<Role> GetRoles();
        List<PropertyType> GetPropertyTypes();
        bool DeleteRole(int RoleId);
        bool DeletePropertyTypes(int propertyType);
        List<Role> EditRoles(Role role);
        List<PropertyType> EditPropertyTypes(PropertyType property);
    }
}

