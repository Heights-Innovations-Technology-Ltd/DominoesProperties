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
            var up = _context.PropertyUploads.Find(uploadId);
            _context.PropertyUploads.Remove(up);
            _context.SaveChanges();

        }

        public List<PropertyUpload> GetUploads(string propertyId)
        {
            return _context.PropertyUploads.Where(x => x.PropertyId.Equals(propertyId)).ToList();
        }

        public bool NewUpload(List<PropertyUpload> uploads)
        {
            try
            {
                _context.PropertyUploads.AddRangeAsync(uploads);
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