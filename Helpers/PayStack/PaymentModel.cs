using System;
using Newtonsoft.Json;

namespace Helpers.PayStack
{
    public class PaymentModel
    {
        [JsonProperty("amount")]
        public decimal amount;
        [JsonProperty("email")]
        public string email;
        [JsonProperty("ref")]
        public string reference;
        [JsonProperty("callback_url")]
        public string callback;
    }
}
