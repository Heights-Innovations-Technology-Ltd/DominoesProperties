using System;
using Models.Models;

namespace Repositories.Repository
{
    public interface IPaystackRepository
    {
        void NewPayment(PaystackPayment paystack);
        void UpdatePayment(PaystackPayment paystack);
        PaystackPayment GetPaystack(string reference);
    }
}
