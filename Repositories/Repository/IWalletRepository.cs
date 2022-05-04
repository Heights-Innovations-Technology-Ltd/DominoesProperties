using Models.Models;

namespace Repositories.Repository
{
    public interface IWalletRepository
    {
        void CreateCustomerWallet(Wallet wallet);
        void UpdateCustomerWallet(Wallet wallet);
        Wallet GetCustomerWallet(long customerId);
        Wallet GetCustomerWallet(string walletId);
    }
}
