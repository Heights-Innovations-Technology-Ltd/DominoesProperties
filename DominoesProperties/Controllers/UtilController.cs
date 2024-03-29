﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository customerRepository;
        private readonly IWebHostEnvironment environment;
        private readonly IPropertyRepository propertyRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly IUtilRepository utilRepository;

        public UtilController(IConfiguration _configuration, IWebHostEnvironment _environment,
            IEmailService emailService, ICustomerRepository _customerRepository,
            IPropertyRepository _propertyRepository, IUtilRepository _utilRepository)
        {
            configuration = _configuration;
            environment = _environment;
            _emailService = emailService;
            customerRepository = _customerRepository;
            propertyRepository = _propertyRepository;
            utilRepository = _utilRepository;
        }

        [HttpPost("enquiry")]
        [Authorize(Roles = "CUSTOMER")]
        public ApiResponse Enquiry([FromBody] Enquiry enquiry)
        {
            var customer = customerRepository.GetCustomer(enquiry.CustomerUniqueReference);
            if (customer == null)
            {
                response.Message = "Invalid customer detected, only a valid and logged in customer can make enquiry";
                return response;
            }

            var property = propertyRepository.GetProperty(enquiry.PropertyReference);
            if (property == null)
            {
                response.Message = "Selected property not found";
                return response;
            }

            if (utilRepository.AddEnquiry(enquiry))
            {
                response.Success = true;
                response.Message =
                    "Your request has been received. One of our agent will be in touch with you shortly.";

                string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\enquiry.html");
                string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));

                string filePath2 = Path.Combine(environment.ContentRootPath, @"EmailTemplates\enquiry-admin.html");
                string html2 = System.IO.File.ReadAllText(filePath2.Replace(@"\", "/"));
                html2 = html2.Replace("{PROPERTY}", property.Name)
                    .Replace("{CUSTOMER}", $"{customer.FirstName} {customer.LastName}")
                    .Replace("{SUBJECT}", enquiry.Subject).Replace("{MESSAGE}", enquiry.Message);

                _ = _emailService.SendEmail(new EmailData
                {
                    EmailBody = html,
                    EmailSubject = "Property Enquiry - Dominoes Society",
                    EmailToName = customer.FirstName,
                    EmailToId = customer.Email
                });

                _ = _emailService.SendEmail(new EmailData
                {
                    EmailBody = html2,
                    EmailSubject = "Property Enquiry - Dominoes Society",
                    EmailToName = "Inquiries",
                    EmailToId = "inquiries@realestatedominoes.com"
                });

                return response;
            }

            return response;
        }

        [HttpGet("enquiry")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse Enquiry([FromQuery] QueryParams queryParams, [FromQuery] string propertyId,
            [FromQuery] string customerId)
        {
            var enquiries = utilRepository.GetEnquiries();
            if (enquiries == null || enquiries.Count < 1)
            {
                response.Success = true;
                response.Message = "No record found";
                response.Data = new List<Enquiry>();
                return response;
            }

            if (!string.IsNullOrEmpty(customerId))
            {
                enquiries = enquiries.Where(x => x.CustomerUniqueReference.Equals(customerId)).ToList();
            }

            if (!string.IsNullOrEmpty(propertyId))
            {
                enquiries = enquiries.Where(x => x.PropertyReference.Equals(propertyId)).ToList();
            }

            PagedList<Enquiry> enqList = PagedList<Enquiry>.ToPagedList(
                enquiries.OrderBy(on => on.DateCreated).AsQueryable(),
                queryParams.PageNumber, queryParams.PageSize);

            //(int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata2 =
            //(
            //    enqList.TotalCount,
            //    enqList.PageSize,
            //    enqList.CurrentPage,
            //    enqList.TotalPages,
            //    enqList.HasNext,
            //    enqList.HasPrevious
            //);

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new JObject(metadata2)));
            response.Success = enqList.Count > 0;
            response.Message = response.Success ? "Successfull" : "No request found";
            response.Data = enqList;
            return response;
        }

        [HttpPut("enquiry/{enquiryId}/{status}")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse Enquiry(long enquiryId, string status)
        {
            try
            {
                var enquiry = utilRepository.GetEnquiry(enquiryId);
                enquiry.Status = Enum.Parse<EnquiryStatus>(status).ToString();
                enquiry.ClosedBy = HttpContext.User.Identity!.Name;
                utilRepository.CloseEnquiry(enquiry);
                response.Message = "Enquiry closed successfully";
                response.Success = true;
                return response;
            }
            catch (Exception)
            {
                response.Message = "Error updating property enquiry";
                return response;
            }
        }

        [HttpGet("enquiry/{enquiryId}")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse Enquiry(long enquiryId)
        {
            var enq = utilRepository.GetEnquiry(enquiryId);
            if (enq != null)
            {
                response.Data = enq;
                response.Success = true;
                response.Message = "Successful";
                return response;
            }

            return response;
        }

        [HttpGet("location")]
        [AllowAnonymous]
        public ApiResponse Location()
        {
            response.Data = propertyRepository.Locations();
            response.Success = true;
            response.Message = "Successful";
            return response;
        }

        [HttpPost("subscribers")]
        public ApiResponse Subscribe(
            [FromBody]
            [Required(ErrorMessage = "Email is required")]
            [MaxLength(100)]
            [EmailAddress(ErrorMessage = "Not a valid email address")]
            string Email)
        {
            Newsletter newsletter = new()
            {
                Email = Email
            };
            var xx = utilRepository.AddSubscibers(newsletter);
            if (xx == 0)
            {
                var filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\newsletter.html");
                var html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));

                _emailService.SendEmail(new EmailData
                {
                    EmailBody = html,
                    EmailSubject = "Newsletter -  Real Estate",
                    EmailToId = Email
                });

                response.Success = true;
                response.Message = "You have been added to our mailing list successfully";
                return response;
            }

            response.Success = false;
            response.Message = xx == 1
                ? "You seem to already have subscribed to our newsletter"
                : "Error saving your email, please try again shortly";
            return response;
        }

        [HttpGet("subscribers")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse Subscribers()
        {
            response.Success = true;
            response.Message = "List fetched successfully";
            response.Data = utilRepository.GetNewsletterSubscibers();
            return response;
        }

        [HttpPost("onboard-customers")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse OnboardCustomers([FromBody] HashSet<NewCustomers> customers)
        {
            if (configuration.GetValue<bool>("app_settings:onboard"))
            {
                if (customers == null)
                {
                    response.Success = false;
                    response.Message = "No customer selected, please select a customer to onboard";
                    return response;
                }

                OnboardCustomers(customers.ToList());

                response.Success = true;
                response.Message = "On-boarding email sending in process";
                return response;
            }

            response.Success = false;
            response.Message = "On-boarding email sending is disabled";
            return response;
        }

        private void OnboardCustomers(IReadOnlyCollection<NewCustomers> customers)
        {
            Task.Run(() => customers.ToList().ForEach(x =>
            {
                var filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\account-setup.html");
                var html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
                html = html.Replace("{FIRSTNAME}", $"{x.FirstName} {x.LastName}".ToUpper())
                    .Replace("{webroot}", configuration["app_settings:WebEndpoint"]).Replace("{USERNAME}", x.Email)
                    .Replace("{PASSWORD}", "Welcome@2022");

                _ = _emailService.SendEmail(new EmailData
                {
                    EmailBody = html,
                    EmailSubject = "Customer On-boarding -  Real Estate Dominoes",
                    EmailToName = $"{x.FirstName} {x.LastName}",
                    EmailToId = x.Email
                });
            }));
        }

        [HttpGet("test-email/{email}/{encoded}")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public string SendTestMail(string email, string encoded)
        {
            var token = CommonLogic.GetUniqueRefNumber("AT");
            var url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"],
                ValidationModule.ACTIVATE_ACCOUNT.ToString().ToLower(), token, "customer");
            var filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\welcome.html");
            var html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{name}", string.Format("{0} {1}", "Dominoes", "Tester")).Replace("{link}", url);

            _ = _emailService.SendEmail(new EmailData
            {
                EmailBody = html,
                EmailSubject = "New Customer -  Real Estate ",
                EmailToName = "Dominoes Tester",
                EmailToId = email
            });

            return CommonLogic.Decrypt(encoded);
        }

        [HttpPost("test/encypt")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse TestEncryption([FromBody] Dictionary<string, string> keyValues)
        {
            Dictionary<string, string> result = new();
            foreach (var keyValuePairs in keyValues)
            {
                result.Add(keyValuePairs.Key,
                    "encrypt".Equals("encrypt")
                        ? CommonLogic.Encrypt(keyValuePairs.Value)
                        : CommonLogic.Decrypt(keyValuePairs.Value));
            }

            response.Success = true;
            response.Message = "Success!";
            response.Data = result;
            return response;
        }

        [HttpGet("all-customer")]
        [Authorize(Roles = "ADMIN, SUPER")]
        public ApiResponse Customers([FromQuery] QueryParams queryParams)
        {
            //PagedList<Customer> customers = customerRepository.GetCustomers(queryParams);
            //(int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata = (
            //     customers.TotalCount,
            //     customers.PageSize,
            //     customers.CurrentPage,
            //     customers.TotalPages,
            //     customers.HasNext,
            //     customers.HasPrevious
            // );

            var customers = customerRepository.GetCustomers();
            List<NewCustomers> custs = new();
            customers.ForEach(x =>
            {
                custs.Add(new NewCustomers
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Phone = x.Phone,
                    IsActive = x.IsActive!.Value,
                    IsSubscribed = x.IsSubscribed!.Value,
                    NextSubDate = x.NextSubscriptionDate.ToString()
                });
            });

            //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            response.Success = true;
            response.Message = "Successful";
            response.Data = custs;
            return response;
        }
    }
}

public class NewCustomers
{
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
    public bool IsSubscribed { get; set; }
    public string NextSubDate { get; set; }
}