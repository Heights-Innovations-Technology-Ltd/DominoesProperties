using Models.Models;

namespace DominoesProperties.Helper
{
    public class ClassConverter
    {
        public static Models.Customer ConvertCustomerToModel()
        {
            return null;
        }

        public static Customer ConvertCustomerToEntity(Models.Customer customer)
        {
            return new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                Password = CommonLogic.Encrypt(customer.Password)
            };
        }

        public static Models.Profile ConvertCustomerToProfile(Customer customer)
        {
            return new Models.Profile {
                UniqueReference = customer.UniqueRef,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                Email = customer.Email,
                IsAccountVerified = customer.IsAccountVerified,
                AccountNumber = customer.AccountNumber,
                IsActive = customer.IsActive,
                IsSubscribed = customer.IsSubscribed,
                IsVerified = customer.IsVerified,
                Phone = customer.Phone,
                WalletId = customer.Wallet.WalletNo,
                WalletBalance = customer.Wallet.Balance.Value
            };
        }
    }
}
