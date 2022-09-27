using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Repositories.Repository;
using SelectPdf;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IWebHostEnvironment environment;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public ReportController(IInvestmentRepository _investmentRepository, ICustomerRepository _customerRepository, IWebHostEnvironment _environment,
            IEmailService _emailService, IConfiguration _configuration)
        {
            investmentRepository = _investmentRepository;
            customerRepository = _customerRepository;
            environment = _environment;
            emailService = _emailService;
            configuration = _configuration;
        }

        [HttpPost]
        [Route("statement")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse generateStatement([FromBody] ReportRequest report)
        {
            var customer = customerRepository.GetCustomer(report.CustomerUniqueId);
            if (customer == null)
            {
                response.Message = "Invalid customer identifier supplied";
                return response;
            }

            var investments = investmentRepository.GetInvestments(customer.Id).Where(x => x.PaymentDate >= report.StartDate && x.PaymentDate <= report.EndDate).ToList();
            var investmentShare = investmentRepository.GetSharingEntries(customer.Id).Where(x => x.Date >= report.StartDate && x.Date <= report.EndDate).ToList();

            if((investments == null || investments.Count == 0) && (investmentShare == null || investmentShare.Count == 0))
            {
                response.Message = "You have no investment transactions for the specified period";
                return response;
            }
            _ = GenerateInvestmentStatementAsync(customer.Id, report.StartDate, report.EndDate, customer, investments, investmentShare);

            response.Success = true;
            response.Message = "Your investment statement has been sent to your email";
            response.Data = null;
            return response;
        }

        private async Task GenerateInvestmentStatementAsync(long customerId, DateTime start, DateTime end, Customer customer, List<Investment> investments,
            List<Sharingentry> investmentShare)
        {
            await Task.Run(() =>
            {
                List<ReportObject> list = new();
                investments.ForEach(x =>
                {
                    ReportObject reportObject = new();
                    reportObject.date = x.PaymentDate;
                    reportObject.InvestmentName = x.Property.Name;
                    reportObject.Units = x.Units.ToString();
                    reportObject.InitPrice = (x.UnitPrice * x.Units).ToString();
                    reportObject.CurrPrice = (x.Property.UnitPrice * x.Units).ToString();

                    list.Add(reportObject);
                });

                investmentShare.ForEach(x =>
                {
                    ReportObject reportObject = new();
                    reportObject.date = x.Date;
                    reportObject.InvestmentName = x.Group.Property.Name;
                    reportObject.Units = $"{x.PercentageShare}%";
                    reportObject.InitPrice = (x.Group.UnitPrice * x.PercentageShare / 100).ToString();
                    reportObject.CurrPrice = (x.Group.UnitPrice * x.Group.Property.UnitPrice).ToString();

                    list.Add(reportObject);
                });

                string tableList = "";
                list.ForEach(x =>
                {
                    tableList +=
                "<tr>" +
                $"<td>{x.date}</td>" +
                $"<td>{x.InvestmentName}</td>" +
                $"<td>{x.Units}</td>" +
                $"<td>{x.InitPrice}</td>" +
                $"<td>{x.CurrPrice}</td>" +
                "</tr>";
                });

                string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\statement.html");
                string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                html = html
                    .Replace("{STARTDATE}", start.Date.ToShortDateString())
                    .Replace("{ENDDATE}", end.Date.ToShortDateString())
                    .Replace("{CUST_NAME}", $"{customer.LastName} {customer.FirstName}")
                    .Replace("{CUST_BAL}", customer.Wallet.Balance.ToString())
                    .Replace("{CUST_DATE}", customer.DateRegistered.Date.ToShortDateString())
                    .Replace("{list-table}", tableList)
                    .Replace("{webroot}", configuration["app_settings:WebEndpoint"]);
                string subject = "Real Estate Dominoes - Account Statement";

                filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\statement-body.html");
                string html2 = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                html2 = html2.Replace("{FIRSTNAME}", customer.FirstName).Replace("{webroot}", configuration["app_settings:WebEndpoint"]);

                var file = CommonLogic.GeneratePDF(html, environment.ContentRootPath, "Customer Statement", $"{customer.UniqueRef}-Statement");

                EmailDataWithAttachment emailData = new()
                {
                    EmailBody = html2,
                    EmailSubject = subject,
                    EmailToId = customer.Email,
                    EmailToName = customer.FirstName,
                    EmailAttachments = file
                };
                return emailService.SendEmailWithAttachment(emailData);
            });
            
        }
    }

    class ReportObject
    {
        public DateTime date { get; set; }
        public string InvestmentName { get; set; }
        public string Units { get; set; }
        public string InitPrice { get; set; }
        public string CurrPrice { get; set; }
    }
}

