using System;
using System.Collections.Generic;
using System.Linq;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class CustomerService : BaseRepository, ICustomerRepository
    {

        public CustomerService(dominoespropertiesContext context):base(context)
        {
        }

        public Customer CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

            Wallet customerWallet = new Wallet
            {
                CustomerId = customer.Id,
                WalletNo = CommonLogic.GetUniqueNumber(customer.Email),
            };

            _context.Wallets.Add(customerWallet);
            _context.SaveChanges();

            return customer;
        }

        public void DeleteCustomer(string uniqueReference)
        {
            var customer = _context.Customers.Where(x => x.UniqueRef.Equals(uniqueReference)).SingleOrDefault();
            customer.IsDeleted = true;
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        public Customer GetCustomer(string email)
        {
            var customer = _context.Customers.Local.Where(x => x.Email.Equals(email)).SingleOrDefault();
            if(customer == null)
            {
                customer = _context.Customers.Where(x => x.Email.Equals(email)).SingleOrDefault();
            }
            return customer;
        }

        public List<Customer> GetCustomers()
        {
            var customers = _context.Customers.Local.ToList();
            if(customers.Count < 1)
            {
                customers = _context.Customers.ToList();
            }
            return customers;
        }

        public Customer UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges(); throw new NotImplementedException();
        }
    }
}
