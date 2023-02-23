using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Models;

namespace Repositories.Repository
{
    public interface IInvestmentRepository
    {
        long AddInvestment(Investment investment);
        void UpdateInvestment(Investment investment);
        List<Investment> GetInvestments(long customerId);
        PagedList<Investment> GetInvestments(QueryParams queryParams);
        Investment GetInvestment(long investmentId);
        Investment GetNewInvestments(string transactionRef);
        IEnumerable<Investment> GetPropertyInvestments(long propertyId);
        bool AddInvestmentFromWallet(Investment investment);
        List<char> GetUsersOnInvestment(long propertyId);
        List<Sharinggroup> GetSharinggroups(long propertyId);
        bool AddSharingentry(Sharingentry sharingentry);
        bool AddSharingGroup(Sharinggroup sharinggroup);
        Task<bool> CloseSharingGroupAsync(long groupId);
        Sharinggroup GetSharinggroups(string groupRef);
        void DeleteGroup(Sharinggroup groupRef);
        void UpdateSharingGroup(Sharinggroup sharinggroup);
        List<Sharinggroup> CompletedSharingGroup();
        List<Sharinggroup> UncompletedSharingGroup();
        IEnumerable<Sharingentry> GetSharingEntries(long customerId);
        void DeletePendingInvestments();

        IEnumerable<OfflineInvestment> GetOfflineInvestments(long customerId);
        OfflineInvestment GetOfflineInvestment(long id);
        bool AddOfflineInvestment(OfflineInvestment investment);
        OfflineInvestment UpdateOfflineInvestment(OfflineInvestment investment);
        OfflineInvestment GetOfflineInvestment(string paymentRef);
        Task<object> GetOfflineInvestments();
        bool GetOpenOfflineInvestments(long customerId, decimal amount, long propertyId);
    }
}