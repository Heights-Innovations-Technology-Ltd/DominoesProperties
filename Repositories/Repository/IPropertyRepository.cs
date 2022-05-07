using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IPropertyRepository
    {
        bool AddNewProperty(Property property);
        Property AddPropertyDescription(Description description);
        Property UpdatePropertyDescription(Description description);
        Property UpdateProperty(Property property);
        bool DeleteProperty(string uniqueId);
        Property GetProperty(string uniqueId);
        List<Property> GetProperties();
    }
}
