using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IAdminRepository
    {
        bool AddUser(Admin user);
        Admin GetUser(string identifier);
        List<Admin> GetUser();
        void DeleteUser(string uniqueReference);
        Admin UpdateUser(Admin customer);
        public Dictionary<string, int> AdminDashboard();
    }
}
