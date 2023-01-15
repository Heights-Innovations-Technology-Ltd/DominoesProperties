using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominoesProperties.Enums;
using DominoesProperties.Helper;
using DominoesProperties.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILoggerManager _logger;
        private readonly ApiResponse _response = new(false, "Error performing request, contact admin");

        public BlogController(IBlogPostRepository blogPostRepository, ILoggerManager logger, IConfiguration config,
            IAdminRepository adminRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _blogPostRepository = blogPostRepository;
            _logger = logger;
            _adminRepository = adminRepository;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        private string GetBlogPhoto(HttpRequest httpRequest)
        {
            return (from formFile in httpRequest.Form.Files
                where formFile.Name.Equals("blogImage")
                select formFile.FileName).FirstOrDefault();
        }

        [Route("posts")]
        [HttpGet]
        [AllowAnonymous]
        public ApiResponse GetBlogPost()
        {
            List<BlogModel> blogModelList = new();
            try
            {
                var blogs = _blogPostRepository.BlogPosts().Where(x => !x.IsDeleted.GetValueOrDefault()).ToList();
                foreach (var blog in blogs)
                {
                    var blogPostModel = ClassConverter.GetBlogModelFromBlogPost(blog);
                    blogPostModel.BlogImage = !string.IsNullOrEmpty(blog.BlogImage)
                        ? $"{_config["app_settings:UIHostURL"]}{_config["app_settings:blogUploadFilePath"]}/{blog.BlogImage}"
                        : "";
                    blogModelList.Add(blogPostModel);
                }

                if (blogModelList.Count > 0)
                {
                    _response.Success = true;
                    _response.Message = "successfully fetched";
                    _response.Data = blogModelList;
                }
                else
                {
                    _response.Message = "No blog post found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
            }

            return _response;
        }

        [Route("post/{postId}")]
        [HttpGet]
        [AllowAnonymous]
        public ApiResponse GetBlogPostById(string postId)
        {
            try
            {
                var blog = _blogPostRepository.BlogPosts()
                    .Find(x => (x.UniqueNumber.Equals(postId)) && !x.IsDeleted.GetValueOrDefault());

                if (blog != null)
                {
                    var blogPostModel = ClassConverter.GetBlogModelFromBlogPost(blog);
                    blogPostModel.BlogImage = !string.IsNullOrEmpty(blog.BlogImage)
                        ? $"{_config["app_settings:HostURL"]}{_config["app_settings:blogUploadFilePath"]}/{blog.BlogImage}"
                        : "";

                    _response.Success = true;
                    _response.Message = "Successful";
                    _response.Data = blogPostModel;
                }
                else
                {
                    _response.Success = false;
                    _response.Message = "No blog post found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
            }

            return _response;
        }

        private List<string> GetUploadedFileName(HttpRequest httpRequest, string id)
        {
            List<string> fileNameListOfCustomerFiles = new();
            foreach (var formFile in httpRequest.Form.Files)
            {
                if (formFile.Length <= 0) continue;
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                    _config["app_settings:blogUploadFilePath"]);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var paddedFileName = $"{id}{formFile.FileName}";
                using (var stream = new FileStream(Path.Combine(filePath, paddedFileName), FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

                if (formFile.Name.Equals("blogImage"))
                    fileNameListOfCustomerFiles.Add(paddedFileName);
            }

            return fileNameListOfCustomerFiles;
        }

        private List<BlogModel> GetBlogPosts()
        {
            return _blogPostRepository.BlogPosts().Where(x => x.IsDeleted == false).ToList()
                .Select(ClassConverter.GetBlogModelFromBlogPost).ToList();
        }

        [Route("new-post")]
        [HttpPost]
        [Authorize]
        public ApiResponse AddBlogPost(BlogModel blogPostModel)
        {
            _response.Success = false;
            _response.Message = "Error adding blog post, try again in a short period";

            if (blogPostModel == null)
            {
                _response.Message = "Empty blog post request";
                return _response;
            }

            if (string.IsNullOrEmpty(blogPostModel.BlogTitle))
            {
                _response.Message = "Blog title is required";
                return _response;
            }

            if (string.IsNullOrEmpty(blogPostModel.BlogContent))
            {
                _response.Message = "Blog contenet cannot be empty";
                return _response;
            }

            blogPostModel.CreatedBy = HttpContext.User.Identity!.Name;

            if (_adminRepository.GetUser(blogPostModel.CreatedBy).RoleFk.GetValueOrDefault().Equals((int)Role.ADMIN))
            {
                _response.Message = "You do not have required permission for this operation";
                return _response;
            }

            try
            {
                var blog = ClassConverter.GetBlogPostFromModel(blogPostModel);
                if (_blogPostRepository.AddBlogPosts(blog))
                {
                    _response.Success = true;
                    _response.Message = "Blog post successfully created";
                    return _response;
                }
            }
            catch (Exception ex)
            {
                var errorData = JsonConvert.SerializeObject(new
                {
                    BlogModel = blogPostModel,
                });
                _logger.LogInfo(ex.StackTrace);
            }

            return _response;
        }

        [Route("post")]
        [HttpPut]
        [Authorize]
        public ApiResponse UpdateBlogPost(BlogModel blogPostModel)
        {
            _response.Success = false;
            _response.Message = "Error updating blog post, try again in a short period";

            if (blogPostModel.UniqueNumber == null)
            {
                _response.Message = "Unique number must be supplied";
                return _response;
            }

            var blog = _blogPostRepository.BlogPosts().Find(x => x.UniqueNumber.Equals(blogPostModel.UniqueNumber));
            if (blog == null) return _response;
            if (!string.IsNullOrEmpty(blogPostModel.BlogContent))
                blog.BlogContent = blogPostModel.BlogContent;
            if (!string.IsNullOrEmpty(blogPostModel.BlogImage))
                blog.BlogImage = blogPostModel.BlogImage;
            if (!string.IsNullOrEmpty(blogPostModel.BlogTags))
                blog.BlogTags = blogPostModel.BlogTags;
            if (!string.IsNullOrEmpty(blogPostModel.BlogTitle))
                blog.BlogTitle = blogPostModel.BlogTitle;

            try
            {
                if (_blogPostRepository.UpdateBlogPosts(blog))
                {
                    _response.Success = true;
                    _response.Message = "Blog post successfully created";
                    return _response;
                }
            }
            catch (Exception ex)
            {
                var errorData = JsonConvert.SerializeObject(new
                {
                    BlogModel = blogPostModel,
                });
                _logger.LogInfo(ex.StackTrace);
            }

            return _response;
        }

        [Route("delete-post/{postId:long}")]
        [HttpDelete]
        [Authorize]
        public ApiResponse DeleteBlog(long postId)
        {
            _response.Success = false;
            _response.Message = "Error adding blog post, try again in a short period";

            try
            {
                if (postId > 0)
                {
                    var blog = _blogPostRepository.BlogPosts()
                        .FirstOrDefault(x => x.Id == postId || x.UniqueNumber.Equals(postId.ToString()));
                    blog!.IsDeleted = true;
                    if (_blogPostRepository.UpdateBlogPosts(blog))
                    {
                        _response.Success = true;
                        _response.Message = "Blog post successfully created";
                        return _response;
                    }
                }
                else
                {
                    _response.Message = "Blogpost id is required";
                    return _response;
                }
            }
            catch (Exception ex)
            {
                var errorData = JsonConvert.SerializeObject(new
                {
                    Id = postId,
                });
                _logger.LogInfo(ex.StackTrace);
            }

            return _response;
        }
    }
}