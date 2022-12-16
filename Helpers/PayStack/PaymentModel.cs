using Newtonsoft.Json;

namespace Helpers.PayStack
{
    public class PaymentModel
    {
        [JsonProperty("amount")] public decimal Amount;

        [JsonProperty("bearer")] public string Bearer;

        [JsonProperty("callback_url")] public string Callback;

        [JsonProperty("email")] public string Email;

        [JsonProperty("ref")] public string Reference;
    }
}