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
            return _context.Paystackpayments.Where(x => x.PaystackRef.Equals(reference)).FirstOrDefault();
        }

        public void NewPayment(PaystackPayment paystack)
        {
            _context.Paystackpayments.Add(paystack);
            _context.SaveChanges();
        }

        public void UpdatePayment(PaystackPayment paystack)
        {
            _context.Paystackpayments.Update(paystack);
            _context.SaveChanges();
        }
    }
}
