using System;
using System.Collections.Generic;
using System.Linq;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class TransactionService : BaseRepository, ITransactionRepository
    {
        public TransactionService(dominoespropertiesContext context):base(context)
        {
        }

        public Transaction GetTransaction(string transactionRef)
        {
            return _context.Transactions.Where(x => x.TransactionRef.Equals(transactionRef)).FirstOrDefault();
        }

        public List<Transaction> GetTransactions()
        {
            return _context.Transactions.ToList();
        }

        public List<Transaction> GetTransactionsForCustomer(long customerId)
        {
            return _context.Transactions.Where(x => x.CustomerId == customerId).ToList();
        }

        public void NewTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
    }
}
