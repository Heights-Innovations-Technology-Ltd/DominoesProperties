using System;
using System.IO;
using System.Web;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using DominoesProperties.Services;
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
        private readonly IEmailService _emailService;
        
        public UtilController(IUtilRepository _utilRepository, IConfiguration _configuration, IWebHostEnvironment _environment, IEmailService emailService)
        {
            utilRepository = _utilRepository;
            configuration = _configuration;
            environment = _environment;
            _emailService = emailService;
        }

        [HttpGet("test/{email}")]
        public bool SendTestMail(string email)
        {
            string token = CommonLogic.GetUniqueRefNumber("AT");
            string url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], ValidationModule.ACTIVATE_ACCOUNT.ToString().ToLower(), token, "customer");
            string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
            string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{name}", string.Format("{0} {1}", "Dominoes", "Tester")).Replace("{link}", url);

            return _emailService.SendEmail(new EmailData
            {
                EmailBody = html,
                EmailSubject = "New Customer -  Real Estate ",
                EmailToName = "Dominoes Tester",
                EmailToId = email
            });
        }
    }
}