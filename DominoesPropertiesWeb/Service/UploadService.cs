using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DominoesPropertiesWeb.Models;
using DominoesPropertiesWeb.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DominoesPropertiesWeb.Service
{
    public class UploadService : IUploadsRepository
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public UploadService(IWebHostEnvironment _hostEnvironment)
        {
            hostEnvironment = _hostEnvironment;
        }

        private readonly string[] fileExtensions = { ".jpg", ".png", ".jpeg", "gif" };

        public async Task<string> UploadCustomerPassportAsync(IFormFile file, string customerUniqueId, HttpRequest request)
        {
            string url = "";
            try
            {
                string path = Path.GetFullPath(Path.Combine(hostEnvironment.WebRootPath, "Uploads/Passport"));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                    FileInfo fileInfo = new(file.FileName);
                    if (!fileExtensions.Contains(fileInfo.Extension.ToLower()))
                    {
                        return string.Empty;
                    }
                    if (file.Length > 0)
                    {
                        string filename = $"{customerUniqueId.Replace("-", "")}{fileInfo.Extension}";

                        using var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        await file.CopyToAsync(fileStream);

                        url = $"{request.HttpContext.Request.Scheme}://{request.HttpContext.Request.Host}{request.HttpContext.Request.PathBase}'Uploads/Passport/'{filename}";
                    }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
            return url;
        }

        public List<PropertyUpload> UploadPropertyImages(List<PropertyFileUpload> uploads, string propertyId, HttpRequest request)
        {
            return UploadImages(uploads, propertyId, request).Result;
        }

        private async Task<List<PropertyUpload>> UploadImages(List<PropertyFileUpload> files, string propertyId,
            HttpRequest request)
        {
            DateTime Dated = DateTime.Now;
            List<PropertyUpload> properties = new();
            try
            {
                string path = Path.GetFullPath(Path.Combine(hostEnvironment.WebRootPath, "Uploads/Property"));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach (PropertyFileUpload prop in files)
                {
                    IFormFile file = prop.File;
                    FileInfo fileInfo = new(file.FileName);
                    if (!fileExtensions.Contains(fileInfo.Extension.ToLower()))
                    {
                        continue;
                    }
                    if (file.Length > 0)
                    {
                        string filename = $"{propertyId.Replace("-", "")}-{DateTime.UnixEpoch}{fileInfo.Extension}";
                        
                        using var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create);
                        await file.CopyToAsync(fileStream);

                        properties.Add(new PropertyUpload
                        {
                            DateUploaded = Dated,
                            ImageName = filename,
                            Url = $"{request.HttpContext.Request.Scheme}://{request.HttpContext.Request.Host}{request.HttpContext.Request.PathBase}'Uploads/Property/'{filename}",
                            UploadType = prop.UploadType.ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
            return properties;
        }
    }
}

