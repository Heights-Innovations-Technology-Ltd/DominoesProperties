using System;
using DominoesProperties.Models;
using Helpers.PayStack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.Repository;


namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilController : Controller
    {
        private readonly IUtilRepository utilRepository;
        private readonly ApiResponse response = new(false, "Error performing request, contact admin");

        public UtilController(IUtilRepository _utilRepository)
        {
            utilRepository = _utilRepository;
        }

        [HttpGet("test")]
        public string converttojson()
        {
            var reff = Guid.NewGuid().ToString();
            PaymentModel m = new()
            {
                amount = 100,
                email = "au@gmail.com",
                reference = reff,
                callback = string.Format("{0}://{1}/{2}/{3}", Request.Scheme, Request.Host, "api/payment/verify-payment", reff)
            };
            return JsonConvert.SerializeObject(m);
        }
    }
}
