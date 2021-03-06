using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IUploadRepository
    {
        bool NewUpload(PropertyUpload[] uploads);
        List<PropertyUpload> GetUploads(string propertyId);
        void DeleteUpload(long uploadId);
    }
}
