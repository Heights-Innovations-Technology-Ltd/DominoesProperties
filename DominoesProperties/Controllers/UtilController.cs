using System;
using DominoesProperties.Models;
using Helpers;
using Helpers.PayStack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            Console.WriteLine(CommonLogic.Encrypt("domino_user"));
            Console.WriteLine(CommonLogic.Encrypt("user.domino@2022!"));
            //return JsonConvert.SerializeObject(m);
            return "";
        }
    }
}