using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class ClientRequest
    {
        [Required] public string ClientName { get; set; }
    }
}