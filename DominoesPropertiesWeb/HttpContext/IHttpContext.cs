using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DominoesPropertiesWeb.HttpContext
{
    public interface IHttpContext
    {
        Task<dynamic> Get(string endpointURL);
        Task<dynamic> Post(string endpointURL, dynamic obj);
        Task<dynamic> Put(string endpointURL, dynamic obj);
        Task<dynamic> PostUpload(string endpointURL, MultipartFormDataContent content);
    }
}
