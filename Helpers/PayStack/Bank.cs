using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class Bank
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Bank" /> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("status")]
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Datum
    {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        /// <value>
        /// The slug.
        /// </value>
        [JsonProperty("slug")]
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the longcode.
        /// </summary>
        /// <value>
        /// The longcode.
        /// </value>
        [JsonProperty("longcode")]
        public string Longcode { get; set; }

        /// <summary>
        /// Gets or sets the gateway.
        /// </summary>
        /// <value>
        /// The gateway.
        /// </value>
        [JsonProperty("gateway")]
        public string Gateway { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pay with bank].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [pay with bank]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("pay_with_bank")]
        public bool PayWithBank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Datum" /> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the is deleted.
        /// </summary>
        /// <value>
        /// The is deleted.
        /// </value>
        [JsonProperty("is_deleted")]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        [JsonProperty("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
