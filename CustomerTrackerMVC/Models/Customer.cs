using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CustomerTrackerMVC.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter the customer's name.")]
        [StringLength(200, ErrorMessage = "The name can be at most {1} characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public Address Address { get; set; } = new Address();

        [StringLength(20, ErrorMessage = "The telephone number can be at most {1} characters.")]
        [RegularExpression(@"^\+?[\d\s\-()]{7,20}$",
            ErrorMessage = "Enter a valid phone number, e.g. 021 123 4567.")]
        [Display(Name = "Telephone number")]
        public string? TelephoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "The contact person's name can be at most {1} characters.")]
        [Display(Name = "Contact person")]
        public string? ContactPersonName { get; set; }

        [StringLength(100, ErrorMessage = "The email address can be at most {1} characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Enter a valid email address, e.g. name@gmail.com")]
        [Display(Name = "Contact email")]
        public string? ContactPersonEmail { get; set; }

        [StringLength(20, ErrorMessage = "The VAT number can be at most {1} characters.")]
        [RegularExpression(@"^[A-Za-z]{0,2}[A-Za-z0-9 .\-]{4,18}$",
            ErrorMessage = "Enter a valid VAT number, e.g. 4123456789 or GB123456789.")]
        [Display(Name = "VAT number")]
        public string? VatNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}