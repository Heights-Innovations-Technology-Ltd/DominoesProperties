﻿using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface ICustomerRepository
    {
        Customer CreateCustomer(Customer user);
        Customer GetCustomer(string identifier);
        List<Customer> GetCustomers();
        void DeleteCustomer(string uniqueReference);
        Customer UpdateCustomer(Customer customer);
        Customer GetCustomer(long id);
        PagedList<Customer> GetCustomers(QueryParams pageParams);
    }
}
