using System;
using Microsoft.AspNetCore.Http;

namespace DominoesProperties.Models
{
    public class EmailDataWithAttachment : EmailData
    {
        public string EmailAttachments { get; set; }
    }
}
