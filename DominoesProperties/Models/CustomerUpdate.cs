using System;
using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class CustomerUpdate
    {
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
    }
}

