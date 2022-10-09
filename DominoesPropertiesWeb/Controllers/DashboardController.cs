using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISession session;
        private readonly IConfiguration _config;
        private readonly IUploadsRepository _uploadRepository;
        private readonly IHttpContext httpContext;
        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public DashboardController(IHttpContextAccessor httpContextAccessor, IConfiguration config, IHttpContext httpContext, IUploadsRepository uploadRepository)
        {
            this.session = httpContextAccessor.HttpContext.Session;
            _config = config;
            url = _config["Base_URL"];
            this.httpContext = httpContext;
            _uploadRepository = uploadRepository;
        }
        public IActionResult Index()
        {
            var userAuth = this.session.GetString("Token");
            if (userAuth == null || userAuth.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }

        public IActionResult Profile()
        {
            var userAuth = this.session.GetString("Token");
            if (userAuth == null || userAuth.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }

        public IActionResult EditProfile()
        {
            var userAuth = this.session.GetString("Token");
            if (userAuth == null || userAuth.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }

        public IActionResult Investments()
        {
            var isAuthAdmin = this.session.GetString("Token");
            if (isAuthAdmin == null || isAuthAdmin.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }
        public IActionResult Enquiry()
        {
            var isAuthAdmin = this.session.GetString("Token");
            if (isAuthAdmin == null || isAuthAdmin.Equals(string.Empty))
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }

        [Route("customer-dashboard")]
        public async Task<JsonResult> CustomerDashboard()
        {
            var res = Task.Run(() => httpContext.Get("Customer/dashboard"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
        
        [Route("admin-dashboard")]
        public async Task<JsonResult> AdminDashboard()
        {
            var res = Task.Run(() => httpContext.Get("Admin/dashboard"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
        
        [Route("get-customer")]
        public async Task<JsonResult> Customer()
        {
            var res = Task.Run(() => httpContext.Get("Customer"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("update-profile")]
        public async Task<JsonResult> EditCustomer([FromBody] dynamic profile)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(profile));

            dynamic obj = new ExpandoObject();

            obj.AccountNumber = Convert.ToString(jObject["AccountNumber"]);
            obj.BankName = Convert.ToString(jObject["BankName"]);
            obj.Phone = Convert.ToString(jObject["Phone"]);
            obj.Address = Convert.ToString(jObject["Address"]);

            var res = Task.Run(() => httpContext.Put("Customer", obj));
            var data = await res.GetAwaiter().GetResult();
            //await Task.WhenAll(res);

            //var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [HttpPost("/upload-picture/{customerId}")]
        public async Task<JsonResult> UploadDoc(string customerId)
        {
            var json = Request.Form.Files;
           
            if (json.Count > 0)
            {
                IFormFile file = json[0];
                var uploadResponse = await _uploadRepository.UploadCustomerPassportAsync(file, customerId, Request);
                var res = Task.Run(() => httpContext.Post("Customer/passport", uploadResponse));
                //var data = await res.GetAwaiter().GetResult();
                await Task.WhenAll(res);
                var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
                return Json(JsonConvert.SerializeObject(data));
            }
            return Json(this.StatusCode(StatusCodes.Status204NoContent, "Empty request body"));
        }

        [Route("/change-password")]
        public async Task<JsonResult> ChangePassword([FromBody] dynamic password)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(password));

            dynamic obj = new ExpandoObject();

            obj.Token = Convert.ToString(jObject["CurrentPassword"]);
            obj.Password = Convert.ToString(jObject["Password"]);
            obj.ConfirmPassword = Convert.ToString(jObject["Confirm"]);

            var res = Task.Run(() => httpContext.Post("Customer/change-password", obj));
            var data = await res.GetAwaiter().GetResult();
            //await Task.WhenAll(res);
            //var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("fund-wallet")]
        public async Task<JsonResult> FundCustomerWallet([FromBody] dynamic amountObj)
        {
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(amountObj));

            dynamic obj = new ExpandoObject();

            obj.Amount = Convert.ToDecimal(jObject["Amount"]);
            obj.Module = 2;

            var res = Task.Run(() => httpContext.Post("Payment", obj));
            var data = await res.GetAwaiter().GetResult();
            //await Task.WhenAll(res);

            //var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }
        public IActionResult NewsSubscribers()
        {
            var isAuthAdmin = this.session.GetString("RoleFK");
            if (isAuthAdmin == null || isAuthAdmin.Equals(string.Empty))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [Route("get-enquiries")]
        public async Task<JsonResult> GetEnquiries()
        {
            var res = Task.Run(() => httpContext.Get("Util/enquiry"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("get-enquiry/{enquiryId}")]
        public async Task<JsonResult> GetEnquiry(int enquiryId)
        {
            var res = Task.Run(() => httpContext.Get("Util/enquiry/" + enquiryId));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("update-enquiry-status/{enquiryId}/{status}")]
        public async Task<JsonResult> GetEnquiry(int enquiryId, string status)
        {
            var res = Task.Run(() => httpContext.Put($"Util/enquiry/{enquiryId}/{status}", null));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            this.session.Clear();
            return Json(true);
        }
    }
}
