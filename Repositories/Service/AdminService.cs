using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class AdminService : BaseRepository, IAdminRepository
    {
        public AdminService(dominoespropertiesContext context) : base(context)
        {
        }

        public bool AddUser(Admin user)
        {
            try
            {
                _context.Admins.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DeleteUser(string email)
        {
            throw new NotImplementedException();
        }

        public Admin GetUser(string email)
        {
            return _context.Admins
                .Include(x => x.RoleFkNavigation)
                .FirstOrDefault(x => x.Email.Equals(email));
        }

        public List<Admin> GetUser()
        {
            return _context.Admins.ToList();
        }

        public Admin UpdateUser(Admin admin)
        {
            _context.Admins.Update(admin);
            _context.SaveChanges();
            return admin;
        }

        public Dictionary<string, int> AdminDashboard()
        {
            Dictionary<string, int> figures = new();
            var investments = _context.Investments.ToList();
            var properties = _context.Properties.ToList();
            var customers = _context.Customers.ToList();
            figures.Add("Investments", investments.Count());
            figures.Add("Properties", properties.Count());
            figures.Add("ActiveProperties",
                properties.Count(x =>
                    "ONGOING_CONSTRUCTION".Equals(x.Status) || "OPEN_FOR_INVESTMENT".Equals(x.Status)));
            figures.Add("Customers", customers.Count());
            figures.Add("NewCustomers", customers.Count(x => x.DateRegistered.Date.Equals(DateTime.Now.Date)));

            return figures;
        }
    }
}