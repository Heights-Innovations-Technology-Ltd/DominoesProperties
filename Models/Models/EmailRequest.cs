using System;
using Models;

namespace Models.Models
{
    public class EmailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
        public string Attachedfile { get; set; }
        public ApplicationSetting Settings { get; set; } = null;

        public EmailRequest(string Subject, string Body, string ToEmail)
        {
            this.Subject = Subject;
            this.Body = Body;
            this.ToEmail = ToEmail;
        }
    }
}
