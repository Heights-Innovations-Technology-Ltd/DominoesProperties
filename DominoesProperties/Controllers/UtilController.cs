using System;
using System.Collections.Generic;
using System.IO;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly IEmailService _emailService;
        
        public UtilController(IConfiguration _configuration, IWebHostEnvironment _environment, IEmailService emailService)
        {
            configuration = _configuration;
            environment = _environment;
            _emailService = emailService;
        }

        [HttpGet("test-email/{email}/{encoded}")]
        public string SendTestMail(string email, string encoded)
        {
            string token = CommonLogic.GetUniqueRefNumber("AT");
            string url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], ValidationModule.ACTIVATE_ACCOUNT.ToString().ToLower(), token, "customer");
            string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
            string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{name}", string.Format("{0} {1}", "Dominoes", "Tester")).Replace("{link}", url);

            _emailService.SendEmail(new EmailData
            {
                EmailBody = html,
                EmailSubject = "New Customer -  Real Estate ",
                EmailToName = "Dominoes Tester",
                EmailToId = email
            });

            return CommonLogic.Decrypt(encoded);
        }

        [HttpGet("test/{action}/{keyValues}")]
        public ApiResponse TestEncryption(string keyValues, string action)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(keyValues));
            foreach (JProperty property in jObject.Properties())
            {
                keyValuePairs.Add(property.Name, "encrypt".Equals(action) ? CommonLogic.Encrypt(property.Value.ToString()) : CommonLogic.Decrypt(property.Value.ToString()));
            }

            response.Success = true;
            response.Message = "Success!";
            response.Data = keyValuePairs;
            return response;
        }
    }
}