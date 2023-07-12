using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface ICustomerRepository
    {
        Customer CreateCustomer(Customer user);

        Thirdpartycustomer CreateThirdPartyCustomer(string email, string phone, string lastName, string firstName,
            int channel);

        Customer GetCustomer(string identifier);
        List<Customer> GetCustomers();
        void DeleteCustomer(string uniqueReference);
        Customer UpdateCustomer(Customer customer);
        void UpdateCustomers(List<Customer> customers);
        Customer GetCustomer(long id);
        PagedList<Customer> GetCustomers(QueryParams pageParams);
    }
}