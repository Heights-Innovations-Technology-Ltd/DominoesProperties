using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Helpers.Enum;
using Newtonsoft.Json.Serialization;
using Helpers.PayStack;

namespace Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class PaystackApiManager
    {
        #region Variables

        /// <summary>
        /// The test key
        /// </summary>
        private readonly string TestKey = "sk_test_4a20080664df282defc8128b161c15701fae1788";
        //sk_live_be0b6ea293a0a4d8d0a4878d8a9ef8038effd51f
        //sk_test_468c58176cb63135e89444f749d16a5d581a1081

        /// <summary>
        /// The resovle account number API
        /// </summary>
        private readonly string ResovleAccountNumberAPI = "https://api.paystack.co/bank/resolve?account_number={0}&bank_code={1}";

        /// <summary>
        /// The get fetch csutomer
        /// </summary>
        private readonly string getFetchCsutomer = "https://api.paystack.co/customer/{0}";

        /// <summary>
        /// The get fetch csutomer
        /// </summary>
        private readonly string checkCardnumber = "https://api.paystack.co/decision/bin/{0}";

        /// <summary>
        /// The post create customer
        /// </summary>
        private readonly string postCreateCustomer = "https://api.paystack.co/customer";

        /// <summary>
        /// The post create transfer recipient
        /// </summary>
        private readonly string postCreateTransferRecipient = "https://api.paystack.co/transferrecipient";

        /// <summary>
        /// The post initiate transfer
        /// </summary>
        private readonly string postInitiateTransfer = "https://api.paystack.co/transfer";

        /// <summary>
        /// The get checkrecipientcode
        /// </summary>
        private readonly string getCheckrecipientcode = "https://api.paystack.co/transferrecipient";

        /// <summary>
        /// The get check balance
        /// </summary>
        private readonly string getCheckBalance = "https://api.paystack.co/balance";

        /// <summary>
        /// The postinitialize transaction
        /// </summary>
        private readonly string postinitializeTransaction = "https://api.paystack.co/transaction/initialize";

        /// <summary>
        /// The get verfiy transaction
        /// </summary>
        private readonly string getVerfiyTransaction = "https://api.paystack.co/transaction/verify/{0}";

        /// <summary>
        /// The bank list API
        /// </summary>
        private readonly string BankListAPI = "https://api.paystack.co/bank";

        /// <summary>
        /// The Charge API for card and account
        /// </summary>
        private readonly string PostChargeAPI = "https://api.paystack.co/charge";

        private readonly string PostChargeAPINew = "https://api.paystack.co/transaction/charge_authorization";

        private readonly string PostSubmitPin = "https://api.paystack.co/charge/submit_pin";

        private readonly string PostSubmitOTP = "https://api.paystack.co/charge/submit_otp";

        private readonly string CurrencyApi = "https://free.currconv.com/api/v7/convert?q={0}_NGN&compact=ultra&apiKey=76aa9f467f112b759b7b";

        #endregion

        #region public methods
        /// <summary>
        /// Gets the bank list.
        /// </summary>
        /// <returns></returns>
        public Bank GetBankList()
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(BankListAPI);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + TestKey);

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response = string.Empty;
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }
                return GetBanks(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string GetCurrencyRate(string currency)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(CurrencyApi, currency));

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response = string.Empty;
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }
                }
                return response;
            }
            catch (Exception)
            {
                return "{}";
            }

        }

        /// <summary>
        /// Checks the bank account number.
        /// </summary>
        /// <param name="AccountNumber">The account number.</param>
        /// <param name="BankCode">The bank code.</param>
        /// <returns></returns>
        public ResponseModel CheckBankAccountNumber(string accountNumber, string bankCode)
        {
            ResponseModel responseModel = new();
            if (!string.IsNullOrEmpty(accountNumber) && !string.IsNullOrEmpty(bankCode))
            {

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(ResovleAccountNumberAPI, accountNumber, bankCode));
                httpWebRequest.Headers.Add("Authorization", "Bearer " + TestKey);
                httpWebRequest.Method = "GET";

                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    string response = string.Empty;
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            response = streamReader.ReadToEnd();
                        }
                    }
                    if (!string.IsNullOrEmpty(response))
                    {
                        var data = JObject.Parse(response);
                        responseModel = JsonConvert.DeserializeObject<ResponseModel>(response);
                        return responseModel;
                    }

                    responseModel.Message = "No Data available for this AccountNumber";
                    responseModel.Status = false;
                    return responseModel;
                }
                catch (WebException ex)
                {
                    using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var data = sr.ReadToEnd();
                        responseModel = JsonConvert.DeserializeObject<ResponseModel>(data);
                        return responseModel;
                    }
                }
            }
            else
            {
                responseModel.Message = "Please provide accountNumber and bankCode";
                responseModel.Status = false;
                responseModel.Data = null;
                return responseModel;
            }
        }
        public ResponseModel CheckCardNumber(long binnumber)
        {
            ResponseModel responseModel = new ResponseModel();
            if (!string.IsNullOrEmpty(Convert.ToString(binnumber)) && Convert.ToString(binnumber).Length == 6)
            {
                try
                {
                    string[] param = { Convert.ToString(binnumber) };
                    responseModel = CallApi(checkCardnumber, "GET", param);
                    responseModel.Message = "No Data available for this Bin";
                    responseModel.Status = false;
                    return responseModel;
                }
                catch (WebException ex)
                {
                    using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var data = sr.ReadToEnd();
                        responseModel = JsonConvert.DeserializeObject<ResponseModel>(data);
                        return responseModel;
                    }
                }
            }
            else
            {
                responseModel.Message = "Please provide accountNumber and bankCode";
                responseModel.Status = false;
                responseModel.Data = null;
                return responseModel;
            }
        }

        /// <summary>
        /// Transfers the money.
        /// </summary>
        /// <param name="firstname">The firstname.</param>
        /// <param name="lastname">The lastname.</param>
        /// <param name="email">The email.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="accountnumber">The accountnumber.</param>
        /// <param name="accountname">The accountname.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="bancode">The bancode.</param>
        /// <param name="customer_code">The customer code.</param>
        /// <param name="transferreceipntcode">The transferreceipntcode.</param>
        /// <returns></returns>
        public TransferMoneyResponseModel TransferMoney(string firstname, string lastname, string email, string phone, string accountnumber, string accountname, decimal amount, string bancode, string customer_code = null, string transferreceipntcode = null)
        {
            ResponseModel responseModel = new ResponseModel();
            TransferMoneyResponseModel transferMoneyResponseModel = new TransferMoneyResponseModel();

            if (string.IsNullOrEmpty(customer_code))
            {
                //create customer 
                string inputJson = JsonConvert.SerializeObject(new PropsJson(email, firstname, lastname, phone));
                responseModel = CallApi(postCreateCustomer, "POST", null, inputJson);
                if (responseModel.Status)
                {
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(responseModel.Data));
                    transferMoneyResponseModel.CustomerCode = jObject["customer_code"].ToString();

                    if (string.IsNullOrEmpty(transferreceipntcode))
                    {
                        string inputJsontransferreceipent = JsonConvert.SerializeObject(new { type = "nuban", name = accountname, account_number = accountnumber, bank_code = bancode });
                        responseModel = CallApi(postCreateTransferRecipient, "POST", null, inputJsontransferreceipent);
                        transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                        transferreceipntcode = transferMoneyResponseModel.TransferRecipient.recipient_code;
                    }
                    else if (!Transferreceipntexits(transferreceipntcode))
                    {
                        string inputJsontransferreceipent = JsonConvert.SerializeObject(new { type = "nuban", name = accountname, account_number = accountnumber, bank_code = bancode });
                        responseModel = CallApi(postCreateTransferRecipient, "POST", null, inputJsontransferreceipent);
                        transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                        transferreceipntcode = transferMoneyResponseModel.TransferRecipient.recipient_code;
                    }
                    else
                    {
                        transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                    }

                    if (responseModel.Status)
                    {
                        //initlize transfer
                        transferMoneyResponseModel = InitiateTransfer(transferMoneyResponseModel, responseModel, amount, transferreceipntcode);
                    }
                    else
                    {
                        transferMoneyResponseModel.status = false;
                        transferMoneyResponseModel.Message = "Transfer Recipient not created in paystack";
                    }
                }
                else
                {
                    transferMoneyResponseModel.status = false;
                    transferMoneyResponseModel.Message = "Customer not created in paystack";
                }

            }
            else
            {
                string[] parameter = { customer_code };
                responseModel = CallApi(getFetchCsutomer, "GET", parameter);
                if (!responseModel.Status)
                {
                    //if that customer code deleted from paystack then create that customer 
                    string inputJsoncreatecustomer = JsonConvert.SerializeObject(new PropsJson(email, firstname, lastname, phone));
                    responseModel = CallApi(postCreateCustomer, "POST", parameter, inputJsoncreatecustomer);
                    if (responseModel.Status)
                    {
                        JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(responseModel.Data));
                        transferMoneyResponseModel.CustomerCode = jObject["customer_code"].ToString();


                        if (string.IsNullOrEmpty(transferreceipntcode))
                        {
                            string inputJsontransferreceipent = JsonConvert.SerializeObject(new { type = "nuban", name = accountname, account_number = accountnumber, bank_code = bancode });
                            responseModel = CallApi(postCreateTransferRecipient, "POST", null, inputJsontransferreceipent);
                            transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                            transferreceipntcode = transferMoneyResponseModel.TransferRecipient.recipient_code;
                        }
                        else if (!Transferreceipntexits(transferreceipntcode))
                        {
                            string inputJsontransferreceipent = JsonConvert.SerializeObject(new { type = "nuban", name = accountname, account_number = accountnumber, bank_code = bancode });
                            responseModel = CallApi(postCreateTransferRecipient, "POST", null, inputJsontransferreceipent);
                            transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                            transferreceipntcode = transferMoneyResponseModel.TransferRecipient.recipient_code;
                        }
                        else
                        {
                            transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                        }

                        if (responseModel.Status)
                        {
                            //initlize transfer
                            transferMoneyResponseModel = InitiateTransfer(transferMoneyResponseModel, responseModel, amount, transferreceipntcode);
                        }
                        else
                        {
                            transferMoneyResponseModel.status = false;
                            transferMoneyResponseModel.Message = "Transfer Recipient not created in paystack";
                        }
                    }
                    else
                    {
                        transferMoneyResponseModel.status = false;
                        transferMoneyResponseModel.Message = "Customer not created in paystack";
                    }

                }
                else
                {
                    //customer code exists in the paystack
                    if (responseModel.Status)
                    {
                        JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(responseModel.Data));
                        transferMoneyResponseModel.CustomerCode = jObject["customer_code"].ToString();

                        if (string.IsNullOrEmpty(transferreceipntcode))
                        {
                            string inputJsontransferreceipent = JsonConvert.SerializeObject(new { type = "nuban", name = accountname, account_number = accountnumber, bank_code = bancode });
                            responseModel = CallApi(postCreateTransferRecipient, "POST", null, inputJsontransferreceipent);
                            transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                            transferreceipntcode = transferMoneyResponseModel.TransferRecipient.recipient_code;
                        }
                        else if (!Transferreceipntexits(transferreceipntcode))
                        {
                            string inputJsontransferreceipent = JsonConvert.SerializeObject(new { type = "nuban", name = accountname, account_number = accountnumber, bank_code = bancode });
                            responseModel = CallApi(postCreateTransferRecipient, "POST", null, inputJsontransferreceipent);
                            transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                            transferreceipntcode = transferMoneyResponseModel.TransferRecipient.recipient_code;
                        }
                        else
                        {
                            transferMoneyResponseModel.TransferRecipient = JsonConvert.DeserializeObject<TransferRecipient>(Convert.ToString(responseModel.Data));
                        }


                        if (responseModel.Status)
                        {
                            //initlize transfer
                            transferMoneyResponseModel = InitiateTransfer(transferMoneyResponseModel, responseModel, amount, transferreceipntcode);
                        }
                        else
                        {
                            transferMoneyResponseModel.status = false;
                            transferMoneyResponseModel.Message = "Transfer Recipient not created in paystack";
                        }
                    }
                    else
                    {
                        transferMoneyResponseModel.status = false;
                        transferMoneyResponseModel.Message = "Customer not created in paystack";
                    }
                }
            }
            return transferMoneyResponseModel;
        }

        /// <summary>
        /// Mobiles the application initialize transaction.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public ResponseModel MobileAppInitTransaction(PaymentModel model)
        {
            string inputJson = JsonConvert.SerializeObject(model);
            return CallApi(postinitializeTransaction, "POST", null, inputJson);
        }

        /// <summary>
        /// Mobiles the application verify transaction.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public ResponseModel MobileAppVerifyTransaction(string reference)
        {
            string[] parameter = { reference };
            return CallApi(getVerfiyTransaction, "GET", parameter);
        }

        /// <summary>
        /// Checks the paystack balance.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public bool CheckPaystackBalance(decimal amount)
        {
            string[] parameter = { };
            ResponseModel responseModel = new();
            responseModel = CallApi(getCheckBalance, "GET", parameter);
            List<CheckBalanceList> checkBalanceLists = JsonConvert.DeserializeObject<List<CheckBalanceList>>(Convert.ToString(responseModel.Data));
            if (checkBalanceLists != null && checkBalanceLists.Count > 0)
            {
                if (checkBalanceLists[0].balance > amount)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        public decimal GetPaystackBalance()
        {
            string[] parameter = { };
            ResponseModel responseModel = new();
            responseModel = CallApi(getCheckBalance, "GET", parameter);
            List<CheckBalanceList> checkBalanceLists = JsonConvert.DeserializeObject<List<CheckBalanceList>>(Convert.ToString(responseModel.Data));
            if (checkBalanceLists != null && checkBalanceLists.Count > 0)
            {
                return checkBalanceLists[0].balance;
            }
            else
            {
                return 0;
            }
        }

        public bool isTestKey()
        {
            if (TestKey.Contains("test"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // DOB must be yyyy--mm-dd formate
        public ResponseModel ChargeAPI(string email, string amount, bool IsCard, string bankCode, string bankAccountNumber, string Dob, int? cardCVV, string cardNumber, string cardExpiryMonth, string cardExpiryYear)
        {

            try
            {
                ResponseModel responseModel = new();
                string inputJson = string.Empty;
                if (IsCard)
                {
                    var parameters = new
                    {
                        email = email,
                        amount = amount,
                        card = new
                        {
                            cvv = cardCVV,
                            number = cardNumber,
                            expiry_month = cardExpiryMonth,
                            expiry_year = cardExpiryYear
                        }
                    };

                    inputJson = JsonConvert.SerializeObject(parameters);
                    responseModel = CallApi(PostChargeAPI, "POST", null, inputJson);
                }
                else
                {
                    var parameters = new
                    {
                        email = email,
                        amount = amount,
                        bank = new
                        {
                            code = bankCode,
                            account_number = bankAccountNumber
                        },
                        birthday = Dob
                    };

                    inputJson = JsonConvert.SerializeObject(parameters);
                    responseModel = CallApi(PostChargeAPI, "POST", null, inputJson);

                }
                return responseModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public ResponseModel ChargeAuthorization(string emails, string amounts, string authorizationCode)
        {
            try
            {
                ResponseModel responseModel = new();
                string inputJson = string.Empty;
                var parameters = new
                {
                    email = emails,
                    amount = amounts,
                    authorization_code = authorizationCode
                };
                inputJson = JsonConvert.SerializeObject(parameters);
                responseModel = CallApi(PostChargeAPINew, "POST", null, inputJson);

                return responseModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ResponseModel VerifyTransaction(string reference)
        {
            try
            {
                ResponseModel responseModel = new();
                string[] parameter = { reference };
                responseModel = CallApi(getVerfiyTransaction, "GET", parameter);

                return responseModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ResponseModel SubmitOTP(string otp, string reference)
        {
            try
            {
                ResponseModel responseModel = new ResponseModel();
                string inputJson = string.Empty;
                var parameters = new
                {

                    otp = otp,
                    reference = reference
                };
                inputJson = JsonConvert.SerializeObject(parameters);
                responseModel = CallApi(PostSubmitOTP, "POST", null, inputJson);
                return responseModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ResponseModel SubmitPin(string pin, string reference)
        {
            try
            {
                ResponseModel responseModel = new();
                string inputJson = string.Empty;
                var parameters = new
                {
                    pin = pin,
                    reference = reference
                };
                inputJson = JsonConvert.SerializeObject(parameters);
                responseModel = CallApi(PostSubmitPin, "POST", null, inputJson);
                return responseModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


        #region Private methods
        /// <summary>
        /// Gets the banks.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        private Bank GetBanks(string jsonString)
        {
            Bank banks = new Bank();
            if (!string.IsNullOrEmpty(jsonString))
            {
                var data = JObject.Parse(jsonString);
                banks = JsonConvert.DeserializeObject<Bank>(jsonString);
            }
            return banks;
        }

        /// <summary>
        /// Transferreceipntexitses the specified transferreceipntcode.
        /// </summary>
        /// <param name="transferreceipntcode">The transferreceipntcode.</param>
        /// <returns></returns>
        private bool Transferreceipntexits(string transferreceipntcode)
        {
            ResponseModel responseModel = new();
            bool yesorno = false;
            string[] parameter = { };
            responseModel = CallApi(getCheckrecipientcode, "GET", parameter);
            List<RecipientsModel> recipientsModels = new();
            recipientsModels = JsonConvert.DeserializeObject<List<RecipientsModel>>(Convert.ToString(responseModel.Data));
            var obj = recipientsModels.FirstOrDefault(x => x.recipient_code == transferreceipntcode && !x.is_deleted);
            if (obj != null)
                yesorno = true;
            return yesorno;
        }

        /// <summary>
        /// Initiates the transfer.
        /// </summary>
        /// <param name="transferMoneyResponseModel">The transfer money response model.</param>
        /// <param name="responseModel">The response model.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="transferreceipntcode">The transferreceipntcode.</param>
        /// <returns></returns>
        private TransferMoneyResponseModel InitiateTransfer(TransferMoneyResponseModel transferMoneyResponseModel, ResponseModel responseModel, decimal amount, string transferreceipntcode)
        {
            string inputjsoninittrasnfer = JsonConvert.SerializeObject(new { source = "balance", amount = (amount * 100), recipient = transferreceipntcode, currency = "NGN" });
            responseModel = CallApi(postInitiateTransfer, "POST", null, inputjsoninittrasnfer);
            if (responseModel.Status)
            {
                transferMoneyResponseModel.InitTrasnferResponse = responseModel.Data;
                transferMoneyResponseModel.InitTrasnferRequest = inputjsoninittrasnfer;

                transferMoneyResponseModel.APIType = (short)APITpye.InitTransfer;
                transferMoneyResponseModel.RequestType = (short)RequestTpye.Post;
                transferMoneyResponseModel.Url = postInitiateTransfer;
                transferMoneyResponseModel.status = true;
            }
            else
            {
                transferMoneyResponseModel.status = false;
                transferMoneyResponseModel.Message = responseModel.Message;
            }

            return transferMoneyResponseModel;
        }

        /// <summary>
        /// Calls the API.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <param name="apitype">The apitype.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="inputjson">The inputjson.</param>
        /// <returns></returns>
        private ResponseModel CallApi(string api, string apitype, string[] args = null, string inputjson = null)
        {
            ResponseModel responseModel = new();
            try
            {
                if (apitype.Equals("GET"))
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(api, args));
                    httpWebRequest.Headers.Add("Authorization", "Bearer " + TestKey);
                    httpWebRequest.Method = apitype;

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    string response = string.Empty;
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            response = streamReader.ReadToEnd();
                        }
                    }
                    if (!string.IsNullOrEmpty(response))
                    {
                        var data = JObject.Parse(response);
                        responseModel = JsonConvert.DeserializeObject<ResponseModel>(response);
                        return responseModel;
                    }
                }
                else
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(api);
                    httpWebRequest.Headers.Add("Authorization", "Bearer " + TestKey);
                    httpWebRequest.Method = apitype;
                    httpWebRequest.ContentType = "application/json";
                    byte[] bytes = Encoding.UTF8.GetBytes(inputjson);


                    using (Stream stream = httpWebRequest.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Close();
                    }
                    using (WebResponse response1 = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        using (StreamReader reader = new(response1.GetResponseStream(), Encoding.UTF8))
                        {
                            var respo = reader.ReadToEnd();
                            responseModel = JsonConvert.DeserializeObject<ResponseModel>(respo);
                        }
                    }
                    return responseModel;
                }
                responseModel.Message = "No Data available for this AccountNumber";
                responseModel.Status = false;

            }
            catch (WebException ex)
            {
                using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var data = sr.ReadToEnd();
                    responseModel = JsonConvert.DeserializeObject<ResponseModel>(data);
                    return responseModel;
                }
            }
            return responseModel;
        }
        #endregion


    }

    internal record PropsJson(string Email, string First_name, string Last_name, string Phone);
}
