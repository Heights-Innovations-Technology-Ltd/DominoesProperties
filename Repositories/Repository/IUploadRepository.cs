using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IUploadRepository
    {
        bool NewUpload(List<Propertyupload> uploads);
        List<Propertyupload> GetUploads(long propertyId);
        void DeleteUpload(long uploadId);
    }
}
