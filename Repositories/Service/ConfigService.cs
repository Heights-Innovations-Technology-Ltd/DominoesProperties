using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<PropertyType> AddPropertyType(PropertyType propertyType)
        {
            PropertyType prop = _context.PropertyTypes.Add(propertyType).Entity;
            await _context.SaveChangesAsync();
            return prop;
        }

        public async Task<Role> AddRole(Role role)
        {
            Role rol = _context.Roles.Add(role).Entity;
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<bool>  DeletePropertyTypes(int propertyType)
        {
            var cc = _context.Properties.Count(x => x.TypeNavigation.Id == propertyType);
            if(cc == 0)
            {
                PropertyType prop = await _context.PropertyTypes.FirstOrDefaultAsync(x => x.Id == propertyType);
                _context.PropertyTypes.Remove(prop);
                await _context.SaveChangesAsync();
            }
            return false;
        }

        public async Task<bool> DeleteRole(int RoleId)
        {
            var cc = _context.Admins.Count(x => x.RoleFk.Value == RoleId);
            if (cc == 0)
            {
                Role role = _context.Roles.FirstOrDefault(x => x.Id == RoleId);
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
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

        public async Task<List<PropertyType>> EditPropertyTypes(PropertyType property)
        {
            _context.PropertyTypes.Update(property);
            await _context.SaveChangesAsync();
            return GetPropertyTypes();
        }

        public async Task<List<Role>> EditRoles(Role role)
        {
            _context.Roles.Update(role);
           await _context.SaveChangesAsync();
            return GetRoles();
        }
    }
}

