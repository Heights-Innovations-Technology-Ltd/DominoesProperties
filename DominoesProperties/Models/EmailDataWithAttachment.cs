using System;
using Microsoft.AspNetCore.Http;

namespace DominoesProperties.Models
{
    public class EmailDataWithAttachment : EmailData
    {
        public IFormFileCollection EmailAttachments { get; set; }
    }
}
