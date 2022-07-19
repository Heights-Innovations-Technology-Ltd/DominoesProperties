using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.Repository;

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
        private readonly ICustomerRepository customerRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IUtilRepository utilRepository;
        public UtilController(IConfiguration _configuration, IWebHostEnvironment _environment, IEmailService emailService, ICustomerRepository _customerRepository,
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
                response.Message = "Your request has been received. One of our agent will be in touch with you shortly.";
                return response;
            }

            string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\Enquiry.html");
            string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
            html = html.Replace("{name}", customer.FirstName);

            _ = _emailService.SendEmail(new EmailData
            {
                EmailBody = html,
                EmailSubject = "Property Enquiry - Dominoes Society",
                EmailToName = customer.FirstName,
                EmailToId = customer.Email
            });

            return response;
        }

        [HttpGet("enquiry")]
        [Authorize(Roles = "SUPER, ADMIN")]
        public ApiResponse Enquiry([FromQuery] QueryParams queryParams, [FromQuery] string propertyId, [FromQuery] string customerId)
        {
            var enquiries = utilRepository.GetEnquiries();

            if (!string.IsNullOrEmpty(customerId))
            {
                enquiries = enquiries.Where(x => x.CustomerUniqueReference.Equals(customerId)).ToList();
            }

            if (!string.IsNullOrEmpty(propertyId))
            {
                enquiries = enquiries.Where(x => x.PropertyReference.Equals(propertyId)).ToList();
            }

            PagedList<Enquiry> enqList = PagedList<Enquiry>.ToPagedList(enquiries.OrderBy(on => on.DateCreated).AsQueryable(),
                queryParams.PageNumber, queryParams.PageSize);

            (int TotalCount, int PageSize, int CurrentPage, int TotalPages, bool HasNext, bool HasPrevious) metadata2 =
            (
                enqList.TotalCount,
                enqList.PageSize,
                enqList.CurrentPage,
                enqList.TotalPages,
                enqList.HasNext,
                enqList.HasPrevious
            );

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new JObject(metadata2)));
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
                enquiry.ClosedBy = HttpContext.User.Identity.Name;
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
                response.Data =
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

        [HttpGet("test-email/{email}/{encoded}")]
        public String SendTestMail(string email, string encoded)
        {
            string token = CommonLogic.GetUniqueRefNumber("AT");
            string url = string.Format("{0}{1}/{2}?value={3}", configuration["app_settings:WebEndpoint"], ValidationModule.ACTIVATE_ACCOUNT.ToString().ToLower(), token, "customer");
            string filePath = Path.Combine(environment.ContentRootPath, @"EmailTemplates\NewCustomer.html");
            string html = System.IO.File.ReadAllText(filePath.Replace(@"\", "/"));
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