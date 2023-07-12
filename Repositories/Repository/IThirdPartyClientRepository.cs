using System.Collections.Generic;
using Models.Models;

namespace Repositories.Repository
{
    public interface IThirdPartyClientRepository
    {
        Thirdpartyclient AddClinet(string clientName);
        List<Thirdpartyclient> GetClients();
        Thirdpartyclient GetClient(int clientId, string apiKey);
        Thirdpartyclient GetClient(int clientId);
    }
}