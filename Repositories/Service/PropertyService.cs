using System;
using System.Collections.Generic;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class PropertyService : BaseRepository, IPropertyRepository
    {
        public PropertyService(dominoespropertiesContext context) : base(context)
        {
        }

        public bool AddNewProperty(Property property)
        {
            throw new NotImplementedException();
        }

        public Property AddPropertyDescription(Description description)
        {
            throw new NotImplementedException();
        }

        public bool DeleteProperty(string uniqueId)
        {
            throw new NotImplementedException();
        }

        public List<Property> GetProperties()
        {
            throw new NotImplementedException();
        }

        public Property GetProperty(string uniqueId)
        {
            throw new NotImplementedException();
        }

        public Property UpdateProperty(Property property)
        {
            throw new NotImplementedException();
        }

        public Property UpdatePropertyDescription(Description description)
        {
            throw new NotImplementedException();
        }
    }
}
