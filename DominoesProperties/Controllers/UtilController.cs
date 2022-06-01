using System;
using System.IO;
using System.Web;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using Helpers;
using Helpers.PayStack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Newtonsoft.Json;
using Repositories.Repository;


namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : Controller
    {
        private readonly IUtilRepository utilRepository;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public UtilController(IUtilRepository _utilRepository, IConfiguration _configuration, IWebHostEnvironment _environment)
        {
            utilRepository = _utilRepository;
            configuration = _configuration;
            environment = _environment;
        }

        [HttpGet("test")]
        public string converttojson()
        {
            string token = "", html = "";
            token = CommonLogic.GetUniqueRefNumber("AT");
            string url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], ValidationModule.ACTIVATE_ACCOUNT.ToString().ToLower(), token, "customer");
            string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
            html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{name}", string.Format("{0} {1}", "Ayotola", "Jinadu")).Replace("{link}", HttpUtility.UrlEncode(url));

            EmailRequest emailRequest = new("New Customer -  Real Estate ", html, "cse080123@gmail.com");
            emailRequest.Settings = new ApplicationSetting
            {
                SettingName = "EmailNotification",
                TestingEmail = "cse080123@gmail.com",
                TestingMode = false
            };
            CommonLogic.SendEmail(emailRequest);

            Console.WriteLine(CommonLogic.Encrypt("domino_user"));
            Console.WriteLine(CommonLogic.Encrypt("user.domino@2022!"));
            //return JsonConvert.SerializeObject(m);
            return "";
        }
    }
}