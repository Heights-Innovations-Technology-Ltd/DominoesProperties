using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DominoesPropertiesWeb.Models
{
    public class PropertyUpload
    {
        public List<IFormFile> passport { get; set; }
    }
}
