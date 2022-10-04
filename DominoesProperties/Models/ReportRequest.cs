using System;
namespace DominoesProperties.Models
{
    public class ReportRequest
    {
        public string CustomerUniqueId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
    }
}

