using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IUtilRepository
    {
        List<PropertyType> GetPropertyTypes();
        List<Enquiry> GetEnquiries();
        bool AddEnquiry(Enquiry enquiry);
        Enquiry GetEnquiry(string customerIdentifier);
        Enquiry GetEnquiry(long id);
        List<string> GetNewsletterSubscibers();
        int AddSubscibers(Newsletter newsletter);
    }
}
