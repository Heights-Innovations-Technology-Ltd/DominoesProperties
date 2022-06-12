using System;
using System.Linq;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class WalletService :BaseRepository, IWalletRepository
    {
        public WalletService(dominoespropertiesContext context):base(context)
        {
        }

        public void CreateCustomerWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
        }

        public Wallet GetCustomerWallet(long customerId)
        {
            var wallet = _context.Wallets.Local.Where(x => x.CustomerId == customerId).SingleOrDefault();
            if(wallet == null)
            {
                wallet = _context.Wallets.Where(x => x.CustomerId == customerId).SingleOrDefault();
            }
            return wallet;
        }

        public Wallet GetCustomerWallet(string walletNo)
        {
            var wallet = _context.Wallets.Local.Where(x => x.WalletNo == walletNo).SingleOrDefault();
            if (wallet == null)
            {
                wallet = _context.Wallets.Where(x => x.WalletNo == walletNo).SingleOrDefault();
            }
            return wallet;
        }

        public void UpdateCustomerWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
    }
}
