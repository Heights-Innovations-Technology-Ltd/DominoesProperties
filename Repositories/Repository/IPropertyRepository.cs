using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IPropertyRepository
    {
        Property AddNewProperty(Property property);
        Description AddPropertyDescription(Description description);
        Description UpdatePropertyDescription(Description description);
        Property UpdateProperty(Property property);
        bool DeleteProperty(string uniqueId);
        Property GetProperty(string uniqueId);
        List<Property> GetProperties();
        PagedList<Property> GetProperties(QueryParams ownerParameters);
        Description GetDescriptionByPropertyId(string propertyId);
    }
}
