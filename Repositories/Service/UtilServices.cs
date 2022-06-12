using System;
using System.Collections.Generic;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class UtilServices : BaseRepository, IUtilRepository
    {
        public UtilServices(dominoespropertiesContext context) : base(context)
        {
        }

        public List<PropertyType> GetPropertyTypes()
        {
            var propertyTypes = _context.PropertyTypes.Local.ToList();
            if (propertyTypes.Count < 1)
            {
                propertyTypes = _context.PropertyTypes.ToList();
            }
            return propertyTypes;
        }
    }
}
