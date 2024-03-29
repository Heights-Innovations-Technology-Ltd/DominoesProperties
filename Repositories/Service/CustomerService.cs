﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class CustomerService : BaseRepository, ICustomerRepository
    {
        private readonly ILoggerManager logger;

        public CustomerService(dominoespropertiesContext context, ILoggerManager _logger) : base(context)
        {
            logger = _logger;
        }

        public Customer CreateCustomer(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();

                Wallet customerWallet = new Wallet
                {
                    CustomerId = customer.Id,
                    WalletNo = CommonLogic.GetUniqueNumber("WA")
                };

                _context.Wallets.Add(customerWallet);
                _context.SaveChanges();

                logger.LogInfo("Customer creation done successfully");
                return customer;
            }
            catch (Exception ex)
            {
                logger.LogInfo("Error creating customer" + ex);
                throw;
            }
        }

        public Thirdpartycustomer GetThirdPartyCustomer(string identifier)
        {
            var customer = _context.Thirdpartycustomers.Local
                .SingleOrDefault(x =>
                    x.Email.Equals(identifier) || x.Phone == identifier);

            return customer;
        }

        public Thirdpartycustomer CreateThirdPartyCustomer(string email, string phone, string lastName,
            string firstName, int channel)
        {
            try
            {
                if (_context.Thirdpartycustomers.Any(x => x.Email.Equals(email) || x.Phone.Equals(phone)))
                    return null;

                var customerEntity = new Thirdpartycustomer()
                {
                    Email = email,
                    Phone = phone,
                    LastName = lastName,
                    FirstName = firstName,
                    Channel = channel
                };

                var f = _context.Thirdpartycustomers.Add(customerEntity).Entity;
                _context.SaveChanges();
                return f;
            }
            catch (Exception ex)
            {
                logger.LogInfo("Error creating customer" + ex);
                throw;
            }
        }

        public void DeleteCustomer(string uniqueReference)
        {
            var customer = _context.Customers.SingleOrDefault(x => x.UniqueRef.Equals(uniqueReference));
            customer.IsDeleted = true;
            _context.Customers.Update(customer);
            _context.SaveChanges();

            logger.LogInfo("Customer deletion done successfully");
        }

        public Customer GetCustomer(string identifier)
        {
            var customer = _context.Customers.Local
                .SingleOrDefault(x =>
                    x.Email.Equals(identifier) || x.UniqueRef.Equals(identifier) || x.Phone == identifier) ?? _context
                .Customers
                .Where(x => x.Email.Equals(identifier) || x.UniqueRef.Equals(identifier))
                .Include(x => x.Wallet)
                .SingleOrDefault();

            return customer;
        }

        public Customer GetCustomer(long id)
        {
            var customer = _context.Customers.Local.FirstOrDefault(x => x.Id == id);
            if (customer == null)
            {
                customer = _context.Customers.FirstOrDefault(x => x.Id == id);
            }

            return customer;
        }

        public List<Customer> GetCustomers()
        {
            var customers = _context.Customers.Local.ToList();
            if (customers.Count < 1)
            {
                customers = _context.Customers.ToList();
            }

            return customers;
        }

        public PagedList<Customer> GetCustomers(QueryParams pageParams)
        {
            return PagedList<Customer>.ToPagedList(_context.Customers.OrderBy(on => on.DateRegistered),
                pageParams.PageNumber,
                pageParams.PageSize);
        }

        public Customer UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return customer;
        }

        public void UpdateCustomers(List<Customer> customers)
        {
            _context.Customers.UpdateRange(customers);
            _context.SaveChanges();
        }
    }
}