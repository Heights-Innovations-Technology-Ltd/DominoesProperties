﻿using System;
using System.Collections.Generic;
using System.Linq;
using Models.Context;
using Models.Models;
using MySql.Data.MySqlClient;
using Repositories.Repository;

namespace Repositories.Service
{
    public class UtilServices : BaseRepository, IUtilRepository
    {
        public UtilServices(dominoespropertiesContext context) : base(context)
        {
        }

        public bool AddEnquiry(Enquiry enquiry)
        {
            try
            {
                _context.Enquiries.Add(enquiry);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Enquiry> GetEnquiries()
        {
            return _context.Enquiries.OrderBy(x => x.DateCreated).ToList();
        }

        public Enquiry GetEnquiry(string customerIdentifier)
        {
            return _context.Enquiries.SingleOrDefault(x => x.CustomerUniqueReference.Equals(customerIdentifier));
        }

        public Enquiry GetEnquiry(long id)
        {
            return _context.Enquiries.SingleOrDefault(x => x.Id == id);
        }

        public List<PropertyType> GetPropertyTypes()
        {
            var propertyTypes = _context.PropertyTypes.Local.ToList();
            if (propertyTypes.Count < 1)
            {
                propertyTypes = _context.PropertyTypes.ToList();
            }
            return propertyTypes;
        }

        public List<string> GetNewsletterSubscibers()
        {
            return _context.Newsletters.Select(x => x.Email).ToList();
        }

        public int AddSubscibers(Newsletter newsletter)
        {
            try
            {
                _context.Newsletters.Add(newsletter);
                _context.SaveChanges();
                return 0;
            }
            catch (MySqlException exception)
            {
                if (exception.Number == 2601 || exception.Message.ToLower().Contains("duplicate"))
                {
                    return 1;
                }
                else
                    return 2;
            }
        }

        public bool CloseEnquiry(Enquiry enquiry)
        {
            try
            {
                _context.Enquiries.Update(enquiry);
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
