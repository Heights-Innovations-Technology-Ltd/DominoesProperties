using System;
using System.Collections.Generic;
using System.Linq;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class PropertyService : BaseRepository, IPropertyRepository
    {
        public PropertyService(dominoespropertiesContext context) : base(context)
        {
        }

        public Property AddNewProperty(Property property)
        {
            _context.Properties.Add(property);
            _context.SaveChanges();
            return property;
        }

        public Property AddPropertyDescription(Description description)
        {
            _context.Descriptions.Add(description);
            _context.SaveChanges();

            return description.Property;
        }

        public bool DeleteProperty(string uniqueId)
        {
            var property = _context.Properties.SingleOrDefault(x => x.UniqueId.Equals(uniqueId));
            property.IsDeleted = true;
            _context.SaveChanges();

            return true;
        }

        public List<Property> GetProperties()
        {
            var properties = _context.Properties.Local.ToList();
            if(properties.Count < 1)
            {
                properties = _context.Properties.ToList();
            }
            return properties;
        }

        public Property GetProperty(string uniqueId)
        {
            var property = _context.Properties.Local.SingleOrDefault(x => x.UniqueId.Equals(uniqueId));
            if (property == null)
            {
                property = _context.Properties.SingleOrDefault(x => x.UniqueId.Equals(uniqueId));
            }
            return property;
        }

        public Property UpdateProperty(Property property)
        {
            _context.Properties.Update(property);
            _context.SaveChanges();

            return property;
        }

        public Property UpdatePropertyDescription(Description description)
        {
            _context.Descriptions.Update(description);
            _context.SaveChanges();

            return description.Property;
        }

        public PagedList<Property> GetProperties(QueryParams pageParams)
        {
            return PagedList<Property>.ToPagedList(_context.Properties.OrderBy(on => on.Id),
                pageParams.PageNumber,
                pageParams.PageSize);
        }
    }
}
