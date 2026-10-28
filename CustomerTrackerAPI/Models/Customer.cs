using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CustomerTrackerAPI.Models
{
    public class Customer
    {
        [Key]
        [Required]
        public int clientId { get; set; }

        [Required(ErrorMessage = "Please enter in the customer's name.")]
        [StringLength(200, ErrorMessage = "The name can be at most {1} characters.")]
        [Display(Name = "Name")]
        public string name { get; set; } = string.Empty;

        public int addressId { get; set; }

        [Required(ErrorMessage = "Please enter in the customer's address.")]
        [Display(Name = "Address")]
        public Address? address { get; set; }

        [StringLength(20, ErrorMessage = "The telephone number can be at most {1} characters.")]
        [RegularExpression(
           @"^(\+[1-9][\d\s]{7,20}|0\d{2}\s?\d{3}\s?\d{4})$",
           ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Telephone number")]
        public string? telephoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email address, e.g. name@gmail.com")]
        [StringLength(100, ErrorMessage = "The email address can be at most {1} characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter a valid email address e.g. name@gmail.com")]
        [Display(Name = "Email Address")]
        public string? email { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "The VAT number can be at most {1} characters.")]
        [RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "The VAT number may only contain letters, digits and dashes.")]
        [Display(Name = "VAT number")]
        public string? vatNumber { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

    }
}
