using System;
using System.Collections.Generic;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class AdminService : BaseRepository, IAdminRepository
    {
        public AdminService(dominoespropertiesContext context):base(context)
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
            return _context.Admins.Find(email);
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
    }
}
