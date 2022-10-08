using System.Collections.Generic;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class ConfigService : BaseRepository, IConfigRepository
    {
        public ConfigService(dominoespropertiesContext context) : base(context)
        {
        }

        public PropertyType AddPropertyType(PropertyType propertyType)
        {
            PropertyType prop = _context.PropertyTypes.Add(propertyType).Entity;
            _context.SaveChanges();
            return prop;
        }

        public Role AddRole(Role role)
        {
            Role rol = _context.Roles.Add(role).Entity;
            _context.SaveChanges();
            return rol;
        }

        public bool DeletePropertyTypes(int propertyType)
        {
            var cc = _context.Properties.Count(x => x.TypeNavigation.Id == propertyType);
            if(cc == 0)
            {
                PropertyType prop = _context.PropertyTypes.FirstOrDefault(x => x.Id == propertyType);
                _context.PropertyTypes.Remove(prop);
                _context.SaveChanges();
            }
            return false;
        }

        public bool DeleteRole(int RoleId)
        {
            var cc = _context.Admins.Count(x => x.RoleFk.Value == RoleId);
            if (cc == 0)
            {
                Role role = _context.Roles.FirstOrDefault(x => x.Id == RoleId);
                _context.Roles.Remove(role);
                _context.SaveChanges();
            }
            return false;
        }

        public List<PropertyType> GetPropertyTypes()
        {
            return _context.PropertyTypes.ToList();
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public List<PropertyType> EditPropertyTypes(PropertyType property)
        {
            _context.PropertyTypes.Update(property);
            _context.SaveChanges();
            return GetPropertyTypes();
        }

        public List<Role> EditRoles(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
            return GetRoles();
        }
    }
}

