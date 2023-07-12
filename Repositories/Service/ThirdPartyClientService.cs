using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Models.Context;
using Models.Models;
using Repositories.Repository;

namespace Repositories.Service
{
    public class ThirdPartyClientService : BaseRepository, IThirdPartyClientRepository
    {
        private readonly ILogger<ThirdPartyClientService> _logger;

        public ThirdPartyClientService(dominoespropertiesContext context, ILogger<ThirdPartyClientService> logger) :
            base(context)
        {
            _logger = logger;
        }

        public Thirdpartyclient AddClinet(string clientName)
        {
            try
            {
                var idd = _context.Thirdpartyclients.ToList();

                var client = new Thirdpartyclient()
                {
                    ClientId = idd.Count < 0 ? 100 : idd.Max(x => x.ClientId) + 1,
                    ApiKey = GenerateKey(),
                    ClientName = clientName
                };

                _context.Thirdpartyclients.Add(client);
                _context.SaveChanges();
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error creating customer", ex);
                throw;
            }
        }

        public List<Thirdpartyclient> GetClients()
        {
            var result = _context.Thirdpartyclients.ToList();
            result.ForEach(x => x.ApiKey = "");
            return result;
        }

        public Thirdpartyclient GetClient(int clientId, string apiKey)
        {
            return _context.Thirdpartyclients.FirstOrDefault(x => x.ClientId == clientId && x.ApiKey == apiKey);
        }

        public Thirdpartyclient GetClient(int clientId)
        {
            return _context.Thirdpartyclients.FirstOrDefault(x => x.ClientId == clientId);
        }

        private static string GenerateKey()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}