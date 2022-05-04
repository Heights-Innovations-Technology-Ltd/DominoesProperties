using System;
using System.ComponentModel.DataAnnotations;

namespace DominoesProperties.Models
{
    public class Profile
    {
        public string UniqueReference { set; get; }

        [Required(ErrorMessage = "Firstname is required")]
        [MaxLength(50, ErrorMessage = "Firstname cannot be more than 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [MaxLength(50, ErrorMessage = "Lastname cannot be more than 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(14)]
        public string Phone { get; set; }

        public string Address { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsVerified { get; set; }

        public bool? IsSubscribed { get; set; }

        public string AccountNumber { get; set; }

        public bool? IsAccountVerified { get; set; }

        public string WalletId { get; set; }

        public decimal WalletBalance { get; set; }
    }
}
