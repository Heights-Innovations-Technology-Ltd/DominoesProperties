using System.Threading.Tasks;

namespace DominoesPropertiesWeb.HttpContext
{
    public interface IHttpContext
    {
        Task<dynamic> Get(string endpointURL);
        Task<dynamic> Post(string endpointURL, dynamic obj);
    }
}
