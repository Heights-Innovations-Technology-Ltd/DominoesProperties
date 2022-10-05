using System;
using DominoesProperties.Models;
using DominoesProperties.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");
        private readonly IEmailService _emailService;
        private readonly ICustomerRepository customerRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IUtilRepository utilRepository;
        public ConfigController(IConfiguration _configuration, IWebHostEnvironment _environment, IEmailService emailService, ICustomerRepository _customerRepository,
        IPropertyRepository _propertyRepository, IUtilRepository _utilRepository)
        {
            configuration = _configuration;
            environment = _environment;
            _emailService = emailService;
            customerRepository = _customerRepository;
            propertyRepository = _propertyRepository;
            utilRepository = _utilRepository;
        }
    }
}

