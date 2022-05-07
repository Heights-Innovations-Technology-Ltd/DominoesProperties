﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Models;
using Newtonsoft.Json;
using SelectPdf;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


/// <summary>
/// 
/// </summary>
public static class CommonLogic
{
    #region Variable Declaration

    /// <summary>
    /// The email regex
    /// </summary>
    public const string EmailRegex = @"([\w\.])+@(?:[\w](?:[\w]*[\w])?\.)+[\w](?:[\w]*[\w])?";
    public const string DisplayDateFormat = "yyyy/MMM/dd";
    /// <summary>
    /// The local email 1
    /// </summary>
    public const string LocalEmail_1 = "jcobsmofe@gmail.com";
    /// <summary>
    /// The local email 2
    /// </summary>
    public const string LocalEmail_2 = "utsav.cmarix@gmail.com";
    /// <summary>
    /// The local email 3
    /// </summary>
    public const string LocalEmail_3 = "sohan.cmarix@gmail.com";
    /// <summary>
    /// The production email 1
    /// </summary>
    public const string ProductionEmail_1 = "seyio@nestbank.ng";
    /// <summary>
    /// The production email 2
    /// </summary>
    public const string ProductionEmail_2 = "dolaposo@nestbank.ng";
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    /// <summary>
    /// The userid
    /// </summary>
    private static long Userid = 0;
    /// <summary>
    /// The test key
    /// </summary>
    private static readonly string TestKey = "sk_test_5cc44481736033b856c6ca404357a3669d0ce234";
    /// <summary>
    /// The live key
    /// </summary>
    private static readonly string LiveKey = "sk_live_a9fe9b10e0e5d007d7872f5e2cda23ec740213b6";


    #endregion

    #region Encrption/Decryption functions
    /// <summary>
    /// Encrypts the specified string text.
    /// </summary>
    /// <param name="strText">The string text.</param>
    /// <returns>
    /// Convert string to base64 string
    /// </returns>
    public static string Encrypt(string strText)
    {
        byte[] byKey = { };
        byte[] IV =
        {
                18,52,86,120,144,171,205,239
            };
        byKey = System.Text.Encoding.UTF8.GetBytes("NestBank");
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(strText);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        string encryptedString = Convert.ToBase64String(ms.ToArray());
        encryptedString = encryptedString.Replace("/", "*");
        encryptedString = encryptedString.Replace("\\", "#");
        encryptedString = encryptedString.Replace("&", "$");
        encryptedString = encryptedString.Replace("+", "_");
        return encryptedString;
    }

    /// <summary>
    /// Decrypts the specified string to decrypt.
    /// </summary>
    /// <param name="stringToDecrypt">The string to decrypt.</param>
    /// <returns>
    /// Convert base64 string to string
    /// </returns>
    public static string Decrypt(string stringToDecrypt)
    {
        stringToDecrypt = stringToDecrypt.Replace("*", "/");
        stringToDecrypt = stringToDecrypt.Replace("#", "\\");
        stringToDecrypt = stringToDecrypt.Replace("$", "&");
        stringToDecrypt = stringToDecrypt.Replace("_", "+");

        byte[] inputByteArray = new byte[stringToDecrypt.Length];
        byte[] byKey = { };
        byte[] IV =
        {
            18,52,86,120,144,171,205,239
            };
        byKey = System.Text.Encoding.UTF8.GetBytes("NestBank");
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        inputByteArray = Convert.FromBase64String(stringToDecrypt);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        return encoding.GetString(ms.ToArray());
    }

    /// <summary>
    /// Sends the SMS.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="toPhone">To phone.</param>
    /// <returns></returns>
    public static string SendSms(string message, string toPhone)
    {
        const string accountSid = "ACb9e4318abc6ce711805e3b08e50d0793";
        const string authToken = "8f3a09d180de856d71cab0132bf60814";

        TwilioClient.Init(accountSid, authToken);

        var messageResponse = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber("+18472609769"),
            to: new Twilio.Types.PhoneNumber(toPhone)
        );

        return messageResponse.ToString();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the unique identifier string.
    /// </summary>
    /// <returns>
    /// return rendom string
    /// </returns>
    public static string GetGUIDString()
    {
        string GuidString = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return GuidString = GuidString.Replace("/", "").Replace("\\", "").Replace("&", "").Replace("+", "");
    }

    /// <summary>
    /// Gets the customer unique number.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static string GetUniqueNumber(string type)
    {
        int _min = 10000000;
        int _max = 99999999;
        Random _rdm = new Random();
        var plainTextBytes = _rdm.Next(_min, _max);
        string random = Convert.ToString(plainTextBytes);
        return string.Format("{0}{1}", type, random.Substring(0, 8));
    }


    /// <summary>
    /// Gets the forgot code.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static string GenCode(string type)
    {
        int _min = 1000;
        int _max = 9999;
        Random _rdm = new Random();
        var plainTextBytes = _rdm.Next(_min, _max);
        string random = Convert.ToString(plainTextBytes);
        return string.Format("{0}{1}", type, random.Substring(0, 4));
    }

    /// <summary>
    /// Gets the unique reference number.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static string GetUniqueRefNumber(string type)
    {
        return string.Format("{0}-{1}", type, string.Format("{0}-{1}", DateTime.Now.ToString("ddMMyyyyhhmmss"), Convert.ToString(GenerateForgotPasswordTokenNumber())));
    }

    /// <summary>
    /// Gets the temporary password for agent.
    /// </summary>
    /// <returns></returns>
    public static string GetTempPasswordForAgent()
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(Guid.NewGuid()));
        string random = System.Convert.ToBase64String(plainTextBytes).Trim().Replace('+', '-').Replace('/', '_').ToUpper();
        return string.Format("{0}", random.Substring(0, 10));
    }

    /// <summary>
    /// Sets the login userid.
    /// </summary>
    /// <param name="userid">The userid.</param>
    public static void SetLoginUserid(long userid)
    {
        Userid = userid;
    }

    /// <summary>
    /// Gets the login userid.
    /// </summary>
    /// <returns></returns>

    public static long GetLoginUserid()
    {
        return Userid;
    }

    /// <summary>
    /// Encodes the card number.
    /// </summary>
    /// <param name="cardnumber">The cardnumber.</param>
    /// <returns></returns>
    public static string EncodeCardNumber(string cardnumber)
    {
        if (!string.IsNullOrEmpty(cardnumber))
        {
            return cardnumber.Substring(0, 3) + " **** **** ****";
        }
        return cardnumber;
    }

    /// <summary>
    /// Generates the PDF.
    /// </summary>
    /// <param name="htmldata">The htmldata.</param>
    /// <param name="path">The path.</param>
    /// <param name="title">The title.</param>
    /// <param name="reportname">The reportname.</param>
    /// <returns></returns>
    public static string GeneratePDF(string htmldata, string path, string title, string reportname)
    {
        try
        {
            string filePath = System.IO.Path.Combine(path, "templates/ReportTemplate.html");
            string templatedate = File.ReadAllText(filePath);
            templatedate = templatedate.Replace("##Title##", title);
            templatedate = templatedate.Replace("##ReportName##", reportname);
            templatedate = templatedate.Replace("##Image##", string.Format("{0}{1}", path, "/images/logo_black.png"));
            templatedate = templatedate.Replace("##Table##", htmldata);
            templatedate = templatedate.Replace("##Date##", DateTime.Now.Date.ToString("MMM/dd/yyyy"));
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MarginLeft = 10;
            converter.Options.MarginTop = 10;
            converter.Options.MarginRight = 10;
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            PdfDocument doc = converter.ConvertHtmlString(templatedate);
            string filename = string.Format("{0}/TempFiles/{1}.pdf", path, reportname);
            doc.Save(filename);
            doc.Close();
            return filename;
        }
        catch (Exception)
        {
            return null;
        }

    }
    /// <summary>
    /// Generates the PDF uni.
    /// </summary>
    /// <param name="htmldata">The htmldata.</param>
    /// <param name="path">The path.</param>
    /// <param name="title">The title.</param>
    /// <param name="reportname">The reportname.</param>
    /// <param name="password">The password.</param>
    /// <param name="pageorientation">The pageorientation.</param>
    /// <returns></returns>
    public static string GeneratePDFUni(string htmldata, string path, string title, string reportname, string password = "", int pageorientation = 0)
    {
        try
        {
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.MarginLeft = 10;
            converter.Options.MarginTop = 10;
            converter.Options.MarginRight = 10;
            if (!string.IsNullOrEmpty(password))
                converter.Options.SecurityOptions.UserPassword = password;

            converter.Options.PdfPageSize = PdfPageSize.A4;
            if (pageorientation == 0)
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            else
                converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            PdfDocument doc = converter.ConvertHtmlString(htmldata);
            string filename = string.Format("{0}/TempFiles/{1}.pdf", path, reportname);
            doc.Save(filename);
            doc.Close();



            return filename;
        }
        catch (Exception)
        {
            return null;
        }

    }

    /// <summary>
    /// Checks the DNS.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="recType">Type of the record.</param>
    /// <returns></returns>
    public static bool checkDNS(string host, string recType = "MX")
    {
        bool result = false;
        try
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "nslookup";
                proc.StartInfo.Arguments = string.Format("-type={0} {1}", recType, host);
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.ErrorDialog = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if ((e.Data != null) && (!result))
                        result = e.Data.StartsWith(host);
                };
                proc.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    { }
                };
                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                proc.WaitForExit(30000);
            }
        }
        catch
        {
            result = false;
        }
        return result;
    }


    //public static bool IsAdmin(long userRoleId)
    //{
    //    if (userRoleId == (short)UserRoleType.Admin)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    //public static bool IsLoanManager(long userRoleId)
    //{
    //    if (userRoleId == (short)UserRoleType.LoanManager)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    //public static bool IsFixedManager(long userRoleId)
    //{
    //    if (userRoleId == (short)UserRoleType.FixedManager)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    //public static bool IsCustomerManager(long userRoleId)
    //{
    //    if (userRoleId == (short)UserRoleType.CustomerManager)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    #endregion

    #region Email 

    /// <summary>
    /// Sends the email.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="body">The body.</param>
    /// <param name="toEmail">To email.</param>
    /// <param name="attachedfile">The attachedfile.</param>
    /// <returns></returns>
    public static string SendEmail(string subject, string body, string toEmail, string attachedfile, ApplicationSetting  settings = null)
    {
        try
        {
            string devEmail = "dev.nestbank.ng@gmail.com";
            MailMessage mail;
            if (settings != null)
            {
                if (settings.TestingMode)
                {
                    mail = new MailMessage(null, settings.TestingEmail);
                }
                else
                {
                    mail = new MailMessage(devEmail, toEmail);
                }
            }
            else
            {
                mail = new MailMessage(devEmail, devEmail);
            }
            mail.Bcc.Add(LocalEmail_3);
            mail.Bcc.Add(LocalEmail_1);
            mail.Bcc.Add(LocalEmail_2);
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("dev.nestbank.ng@gmail.com", "PASSWORDDEV+77");
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = "smtp.gmail.com";

            if (!string.IsNullOrEmpty(attachedfile))
            {
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(attachedfile);
                mail.Attachments.Add(attachment);
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            client.Send(mail);

        }
        catch (Exception ex)
        {
            string errorData = JsonConvert.SerializeObject(new
            {
                _subject = subject,
                _body = body,
                _toEmail = toEmail
            });
            writeLog(ex.Message, errorData);
            return ex.Message;
        }
        return string.Empty;
    }

    public static void SendExceptionEmail(string subject, string body)
    {
        try
        {
            string devEmail = "dev.nestbank.ng@gmail.com";
            MailMessage mail;
            mail = new MailMessage(devEmail, "parthm.cmarix@gmail.com");
            mail.CC.Add("dev.nestbank.ng@gmail.com");
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("dev.nestbank.ng@gmail.com", "PASSWORDDEV+77");
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = "smtp.gmail.com";
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            client.Send(mail);

        }
        catch (Exception ex)
        {

        }
    }


    /// <summary>
    /// Does the payment.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="keyValuePairs">The key value pairs.</param>
    /// <returns></returns>
    //public static string DoPayment(int amount, Dictionary<string, string> keyValuePairs)
    //{
    //    try
    //    {
    //        var testOrLiveSecret = TestKey;
    //        var api = new PayStackApi(testOrLiveSecret);
    //        TransactionInitializeRequest request = new TransactionInitializeRequest();
    //        request.AmountInKobo = (amount);
    //        request.Email = "parthm.cmarix@gmail.com";
    //        List<CustomField> customFields = new List<CustomField>();
    //        foreach (var obj in keyValuePairs)
    //        {
    //            CustomField customField = new CustomField(obj.Key.ToString(), obj.Key.ToString(), obj.Value.ToString());
    //            customFields.Add(customField);
    //        }
    //        request.CustomFields = customFields;
    //        var response = api.Transactions.Initialize(request);
    //        if (response.Status)
    //        {
    //            return Convert.ToString(response.RawJson);
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //    return string.Empty;
    //}

    /// <summary>
    /// Justs the initialize.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="email">The email.</param>
    /// <returns></returns>
    //public static string JustInit(decimal amount, string email)
    //{
    //    try
    //    {
    //        var testOrLiveSecret = LiveKey;
    //        var api = new PayStackApi(testOrLiveSecret);
    //        TransactionInitializeRequest request = new TransactionInitializeRequest();
    //        request.AmountInKobo = Convert.ToInt32(amount);
    //        request.Email = email;
    //        var response = api.Transactions.Initialize(request);
    //        if (response.Status)
    //        {
    //            return Convert.ToString(response.RawJson);
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //    return string.Empty;
    //}

    /// <summary>
    /// Verifies the payment.
    /// </summary>
    /// <param name="reference">The reference.</param>
    /// <returns></returns>
    //public static string VerifyPayment(string reference)
    //{
    //    try
    //    {
    //        var testOrLiveSecret = TestKey;
    //        var api = new PayStackApi(testOrLiveSecret);
    //        var verifyResponse = api.Transactions.Verify(reference);
    //        if (verifyResponse.Status)
    //        {
    //            return Convert.ToString(verifyResponse.RawJson);
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //    return string.Empty;
    //}

    #endregion

    #region Forgot password token

    /// <summary>
    /// Generates the forgot password token number.
    /// </summary>
    /// <returns></returns>
    public static int GenerateForgotPasswordTokenNumber()
    {
        int _min = 1000;
        int _max = 9999;
        Random _rdm = new Random();
        return _rdm.Next(_min, _max);
    }

    /// <summary>
    /// Numbers the words wrapper.
    /// </summary>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    public static String NumWordsWrapper(double n)
    {
        string words = string.Empty;
        double intPart;
        double decPart = 0;
        if (n == 0)
            return "zero";
        try
        {
            string[] splitter = Convert.ToString(n).Split('.');
            intPart = double.Parse(splitter[0]);
            decPart = double.Parse(splitter[1]);
        }
        catch
        {
            intPart = n;
        }

        words = NumWords(intPart);

        if (decPart > 0)
        {
            if (!string.IsNullOrEmpty(words))
                words += " and ";
            int counter = Convert.ToString(decPart).Length;
            switch (counter)
            {
                case 1: words += NumWords(decPart) + " tenths"; break;
                case 2: words += NumWords(decPart) + " hundredths"; break;
                case 3: words += NumWords(decPart) + " thousandths"; break;
                case 4: words += NumWords(decPart) + " ten-thousandths"; break;
                case 5: words += NumWords(decPart) + " hundred-thousandths"; break;
                case 6: words += NumWords(decPart) + " millionths"; break;
                case 7: words += NumWords(decPart) + " ten-millionths"; break;
            }
        }
        return words;
    }
    /// <summary>
    /// Numbers the words.
    /// </summary>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    public static String NumWords(double n)
    {
        string[] numbersArr = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        string[] tensArr = new string[] { "Twenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninty" };
        string[] suffixesArr = new string[] { "Thousand", "Million", "Billion", "Trillion", "Quadrillion", "Quintillion", "Sextillion", "Septillion", "Octillion", "Nonillion", "Decillion", "Undecillion", "Duodecillion", "Tredecillion", "Quattuordecillion", "Quindecillion", "Sexdecillion", "Septdecillion", "Octodecillion", "Novemdecillion", "Vigintillion" };
        string words = string.Empty;

        bool tens = false;

        if (n < 0)
        {
            words += "negative ";
            n *= -1;
        }

        int power = (suffixesArr.Length + 1) * 3;

        while (power > 3)
        {
            double pow = Math.Pow(10, power);
            if (n >= pow)
            {
                if (n % pow > 0)
                {
                    words += NumWords(Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1] + ", ";
                }
                else if (n % pow == 0)
                {
                    words += NumWords(Math.Floor(n / pow)) + " " + suffixesArr[(power / 3) - 1];
                }
                n %= pow;
            }
            power -= 3;
        }
        if (n >= 1000)
        {
            if (n % 1000 > 0) words += NumWords(Math.Floor(n / 1000)) + " thousand, ";
            else words += NumWords(Math.Floor(n / 1000)) + " thousand";
            n %= 1000;
        }
        if (0 <= n && n <= 999)
        {
            if ((int)n / 100 > 0)
            {
                words += NumWords(Math.Floor(n / 100)) + " hundred";
                n %= 100;
            }
            if ((int)n / 10 > 1)
            {
                if (!string.IsNullOrEmpty(words))
                    words += " ";
                words += tensArr[(int)n / 10 - 2];
                tens = true;
                n %= 10;
            }

            if (n < 20 && n > 0)
            {
                if (string.IsNullOrEmpty(words) && tens == false)
                    words += " ";
                words += (tens ? "-" + numbersArr[(int)n - 1] : numbersArr[(int)n - 1]);
                n -= Math.Floor(n);
            }
        }

        return words;

    }

    /// <summary>
    /// Gets the currency format.
    /// </summary>
    /// <param name="CurrencyFormat">The currency format.</param>
    /// <returns></returns>
    public static string GetCurrencyFormat(string CurrencyFormat)
    {
        return (CurrencyFormat + "###,###,##0.00").Trim();
    }

    public static void writeLog(string error, string errorData = null)
    {
        if (!string.IsNullOrEmpty(error))
        {
            string format = "Error at {0} \n Exception Message {1} \n Function Data {2}";
            string.Format(format, DateTime.Now, error, errorData);
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            string foldername = DateTime.Now.Date.ToString("dd_MM_yyyy");
            if (!Directory.Exists(string.Format("{0}/{1}", startupPath, foldername)))
            {
                Directory.CreateDirectory(string.Format("{0}/{1}", startupPath, foldername));
            }
            string filename = string.Format("{0}.{1}", DateTime.Now.Date.ToString("dd_MM_yyyy"), "txt");
            File.AppendAllText(string.Format("{0}/{1}/{2}", startupPath, foldername, filename), error + Environment.NewLine);
        }
    }

    public static long ConvertToTimestamp(DateTime value)
    {
        TimeSpan elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalSeconds;
    }
    #endregion
}