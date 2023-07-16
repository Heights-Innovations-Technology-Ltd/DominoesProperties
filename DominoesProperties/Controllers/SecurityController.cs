using DominoesProperties.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repository;

namespace DominoesProperties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController
    {
        private readonly IThirdPartyClientRepository _clientRepository;
        private readonly ApiResponse _response = new(false, "Error performing request, contact admin");

        public SecurityController(IThirdPartyClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("create-client")]
        public ApiResponse CreateClient([FromBody] ClientRequest clientName, [FromHeader] int clientId,
            [FromHeader] string apiKey)
        {
            if (clientId != 100)
            {
                _response.Success = false;
                _response.Message =
                    $"Forbidden!: You are not allowed on this service";
                return _response;
            }

            var client = _clientRepository.GetClient(clientId, apiKey);
            if (client == null)
            {
                _response.Success = false;
                _response.Message =
                    $"Unauthorized access: Invalid API key supplied";
                return _response;
            }

            var clientCreate = _clientRepository.AddClinet(clientName.ClientName);
            _response.Success = true;
            _response.Message =
                $"Client added successfully with name '{clientName.ClientName}'";
            _response.Data = clientCreate;
            return _response;
        }

        [HttpGet("get-client/{clientId:int}")]
        public ApiResponse CreateClient([FromRoute] int clientId)
        {
            var client = _clientRepository.GetClient(clientId);
            if (client == null)
            {
                _response.Success = false;
                _response.Message =
                    $"Unauthorized access: Invalid API key supplied";
                return _response;
            }

            _response.Success = true;
            _response.Message =
                $"Client fetched successfully";
            _response.Data = client;
            return _response;
        }
    }
}