using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface ICustomerRepository
    {
        bool CreateCustomer(Customer user);
        Customer GetCustomer(string email);
        List<Customer> GetCustomers();
        void DeleteCustomer(string uniqueReference);
        Customer UpdateCustomer(Customer customer);
    }
}
