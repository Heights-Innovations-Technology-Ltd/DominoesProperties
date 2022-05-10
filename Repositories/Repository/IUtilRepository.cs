using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IUtilRepository
    {
        List<PropertyType> GetPropertyTypes();
    }
}
