using DominoesPropertiesWeb.HttpContext;
using DominoesPropertiesWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ISession session;
        private readonly IHttpContext httpContext;
        private readonly IWebHostEnvironment hostEnvironment;
        string url = string.Empty;
        dynamic jsonObj = new JObject();

        public AdminController( IWebHostEnvironment _hostEnvironment, IConfiguration config, IHttpContextAccessor httpContextAccessor, IHttpContext _httpContext)
        {
            _config = config;
            url = _config["Base_URL"];
            this.session = httpContextAccessor.HttpContext.Session;
            httpContext = _httpContext;
            hostEnvironment = _hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Blogs()
        {
            return View();
        }
        public IActionResult CreateBlog()
        {
            return View();
        }

        [Route("/authadmin")]
        public async Task<JsonResult> Auth([FromBody] Login login)
        {
            var res = Task.Run(() => httpContext.Post("Admin/login", login));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;

            bool success = Convert.ToBoolean(data["success"]);

            if (success)
            {
                var resObj = JsonConvert.DeserializeObject<JObject>(Convert.ToString(data["data"]));
                this.session.SetString("Email", (string)resObj["email"]);
                this.session.SetString("RoleFK", (string)resObj["roleFk"]);
                this.session.SetString("IsActive", (string)resObj["isActive"]);
                this.session.SetString("Token", (string)data["TokenObj"]);
                this.session.SetString("Passport", resObj["passportUrl"] != null ? (string)resObj["passportUrl"] : "/images/agents/agent-1.jpeg");
                jsonObj.success = success;
                jsonObj.data = data["message"];
            }
            else
            {
                jsonObj.success = success;
                jsonObj.data = data["data"];
            }
            return Json(JsonConvert.SerializeObject(jsonObj));
        }

        [HttpPost("/new-blog")]
        public async Task<JsonResult> CreateNewBlog()
        {
            var json = Request.Form["params"];

            JObject jObject = JsonConvert.DeserializeObject<JObject>(Convert.ToString(json));

            if (((JObject)jObject).Count > 0)
            {
                string filename = string.Empty;
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var customerfilename in GetUploadedFileName(Request, "BlogUpload"))
                    {
                        filename = customerfilename;
                    }

                }
                dynamic obj = new ExpandoObject();
                obj.BlogTitle = Convert.ToString(jObject["BlogTitle"]);
                obj.BlogContent = Convert.ToString(jObject["BlogContent"]);
                obj.BlogTags = Convert.ToString(jObject["BlogTags"]);
                obj.CreatedBy = Convert.ToString(jObject["CreatedBy"]);
                obj.BlogImage = filename;
                var res = Task.Run(() => httpContext.Post("Blog/new-post", obj));
                var data = await res.GetAwaiter().GetResult();
                return Json(JsonConvert.SerializeObject(data));
            }
            return Json(this.StatusCode(StatusCodes.Status204NoContent, "Empty request body"));
        }
        [Route("/getblogs")]
        public async Task<JsonResult> GetBlogs()
        {
            var res = Task.Run(() => httpContext.Get("BlogPost/posts"));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));

        }

        [Route("/getBlogById/{blogId}")]
        public async Task<JsonResult> GetSingleBlog([FromRoute] string blogId)
        {
            var res = Task.Run(() => httpContext.Get("BlogPost/post/" + blogId));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));

        }

        [Route("/deleteBlog/{blogId}")]
        public async Task<JsonResult> DeleteBlog([FromRoute] string blogId)
        {
            var res = Task.Run(() => httpContext.Delete("BlogPost/delete-post/" + blogId));
            await Task.WhenAll(res);
            var data = res.Status == TaskStatus.RanToCompletion ? res.Result : null;
            return Json(JsonConvert.SerializeObject(data));

        }

        private List<string> GetUploadedFileName(HttpRequest httpRequest, string path)
        {
            List<string> filenamelistofCustomerFiles = new();
            foreach (var formfile in httpRequest.Form.Files)
            {
                if (formfile.Length > 0)
                {
                    string filePath = Path.Combine(hostEnvironment.WebRootPath, _config["app_settings:" + path]);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    string fileName = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + '_' + formfile.FileName;
                    using (var stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                    {
                        formfile.CopyTo(stream);
                    }
                    filenamelistofCustomerFiles.Add(fileName);
                }
            }
            return filenamelistofCustomerFiles;
        }
    }
}
