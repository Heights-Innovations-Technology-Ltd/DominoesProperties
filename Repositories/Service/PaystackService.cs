using System;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class PaystackService : BaseRepository, IPaystackRepository
    {
        public PaystackService(dominoespropertiesContext context): base(context)
        {

        }

        public PaystackPayment GetPaystack(string reference)
        {
            return _context.PaystackPayments.Where(x => x.TransactionRef.Equals(reference)).FirstOrDefault();
        }

        public void NewPayment(PaystackPayment paystack)
        {
            _context.PaystackPayments.Add(paystack);
            _context.SaveChanges();
        }

        public void UpdatePayment(PaystackPayment paystack)
        {
            _context.PaystackPayments.Update(paystack);
            _context.SaveChanges();
        }
    }
}
