using System;
using System.Collections.Generic;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class UploadService : BaseRepository, IUploadRepository
    {
        public UploadService(dominoespropertiesContext context): base(context)
        {
        }

        public void DeleteUpload(long uploadId)
        {
            var up = _context.Propertyuploads.Find(uploadId);
            _context.Propertyuploads.Remove(up);
            _context.SaveChanges();

        }

        public List<Propertyupload> GetUploads(long propertyId)
        {
            return _context.Propertyuploads.Where(x => x.PropertyId.Equals(propertyId)).ToList();
        }

        public bool NewUpload(List<Propertyupload> uploads)
        {
            try
            {
                _context.Propertyuploads.AddRangeAsync(uploads);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}