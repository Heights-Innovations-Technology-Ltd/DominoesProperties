using System.Collections.Generic;
using System.Threading.Tasks;
using DominoesPropertiesWeb.Models;
using Microsoft.AspNetCore.Http;

namespace DominoesPropertiesWeb.Repository
{
    public interface IUploadsRepository
    {
        List<PropertyUpload> UploadPropertyImages(List<PropertyFileUpload> uploads, string propertyId, HttpRequest request);
        Task<string> UploadCustomerPassportAsync(IFormFile file, string customerUniqueId, HttpRequest request);
    }
}

