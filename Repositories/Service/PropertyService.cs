﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class PropertyService : BaseRepository, IPropertyRepository
    {
        public PropertyService(dominoespropertiesContext context) : base(context)
        {
        }

        public Property AddNewProperty(Property property)
        {
            var id = _context.Properties.Max(x => x.Id);
            property.ThirdPartyId = (id + 1).ToString().PadLeft(7, '0');
            _context.Properties.Add(property);
            _context.SaveChanges();
            return property;
        }

        public Description AddPropertyDescription(Description description)
        {
            _context.Descriptions.Add(description);
            _context.SaveChanges();

            return description;
        }

        public bool DeleteProperty(string uniqueId)
        {
            var property = _context.Properties.SingleOrDefault(x => x.UniqueId.Equals(uniqueId));
            property.IsDeleted = true;
            _context.SaveChanges();

            return true;
        }

        public List<Property> GetProperties()
        {
            return _context.Properties.Where(x => !x.IsDeleted.Value && x.UnitAvailable > 0)
                .Include(x => x.PropertyUploads.Where(x => x.UploadType.Equals("COVER")))
                .Include(x => x.TypeNavigation)
                .ToList();
        }

        public Property GetProperty(string uniqueId)
        {
            var property =
                _context.Properties.Local.SingleOrDefault(x =>
                    x.UniqueId.Equals(uniqueId) || x.ThirdPartyId.Equals(uniqueId)) ??
                _context.Properties.Include(x => x.TypeNavigation).SingleOrDefault(x =>
                    x.UniqueId.Equals(uniqueId) || x.ThirdPartyId.Equals(uniqueId));
            return property;
        }

        public Property GetProperty(long uniqueId)
        {
            var property = _context.Properties.Local.SingleOrDefault(x => x.Id == uniqueId);
            if (property == null)
            {
                property = _context.Properties.SingleOrDefault(x => x.Id == uniqueId);
            }

            return property;
        }

        public Description GetDescriptionByPropertyId(string propertyId)
        {
            var description = _context.Descriptions.Local.SingleOrDefault(x => x.PropertyId.Equals(propertyId));
            if (description == null)
            {
                description = _context.Descriptions.SingleOrDefault(x => x.PropertyId.Equals(propertyId));
            }

            return description;
        }

        public Property UpdateProperty(Property property)
        {
            _context.Properties.Update(property);
            _context.SaveChanges();

            return property;
        }

        public Description UpdatePropertyDescription(Description description)
        {
            _context.Descriptions.Update(description);
            _context.SaveChanges();

            return description;
        }

        public PagedList<Property> GetProperties(QueryParams pageParams)
        {
            return PagedList<Property>.ToPagedList(
                _context.Properties.Include(x => x.TypeNavigation).OrderBy(on => on.Id),
                pageParams.PageNumber,
                pageParams.PageSize);
        }

        public List<string> Locations()
        {
            return _context.Properties.Select(x => x.Location).Distinct().ToList();
        }
    }
}