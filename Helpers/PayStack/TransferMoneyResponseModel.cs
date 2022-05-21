using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public class TransferMoneyResponseModel
    {
        public string CustomerCode { get; set; }
        public string Message { get; set; }
        public bool status { get; set; }
        public TransferRecipient TransferRecipient { get; set; }
        public object InitTrasnferResponse { get; set; }
        public object InitTrasnferRequest { get; set; }

        public string Url { get; set; }
        public short APIType { get; set; }
        public short RequestType { get; set; }
    }

    public class TransferRecipient
    {

        public string currency { get; set; }
        public string recipient_code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public details details { get; set; }
        
        public bool active { get; set; }
        public long id { get; set; }
        
    }

    public class details
    {
        public string account_number { get; set; }
        public string account_name { get; set; }
        public string bank_code { get; set; }
        public string bank_name { get; set; }

    }
}
