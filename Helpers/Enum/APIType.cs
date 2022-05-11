using System;
namespace Helpers.Enum
{
    public enum APITpye
    {
        /// <summary>
        /// The create customer
        /// </summary>
        CreateCustomer = 1,
        /// <summary>
        /// The create transfer recipient
        /// </summary>
        CreateTransferRecipient = 2,
        /// <summary>
        /// The initialize transfer
        /// </summary>
        InitTransfer = 3
    }

    public enum RequestTpye
    {
        /// <summary>
        /// The get
        /// </summary>
        GET = 1,
        /// <summary>
        /// The post
        /// </summary>
        Post = 2
    }
}
