using System;
using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface ITransactionRepository
    {
        Transaction GetTransaction(string transactionRef);
        List<Transaction> GetTransactionsForCustomer(long customerId);
        List<Transaction> GetTransactions();
        void NewTransaction(Transaction transaction);
    }
}
