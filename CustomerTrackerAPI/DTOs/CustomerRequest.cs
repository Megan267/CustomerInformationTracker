using System.ComponentModel.DataAnnotations;

namespace CustomerTrackerAPI.DTOs
{
    public class CustomerRequest
    {
        [Required(ErrorMessage = "Please enter the customer's name.")]
        [StringLength(200, ErrorMessage = "The name can be at most {1} characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the customer's address.")]
        public AddressRequest Address { get; set; } = null!;

        [StringLength(20, ErrorMessage = "The telephone number can be at most {1} characters.")]
        [RegularExpression(@"^\+?(?:[\s\-()]{0,2}\d){7,15}$",
            ErrorMessage = "Enter a valid phone number, e.g. 021 123 4567.")]
        public string? TelephoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "The contact person's name can be at most {1} characters.")]
        public string? ContactPersonName { get; set; }

        [StringLength(100, ErrorMessage = "The email address can be at most {1} characters.")]
        [RegularExpression(@"^[A-Za-z0-9_%+\-]+(?:\.[A-Za-z0-9_%+\-]+)*@[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?)*\.[A-Za-z]{2,}$",
            ErrorMessage = "Enter a valid email address, e.g. name@gmail.com")]
        public string? ContactPersonEmail { get; set; }

        [StringLength(20, ErrorMessage = "The VAT number can be at most {1} characters.")]
        [RegularExpression(@"^(?:[A-Za-z]{2} ?)?\d(?:[ .\-]?\d){7,17}$",
        ErrorMessage = "Enter a valid VAT number, e.g. 4123456789 or GB123456789.")]
        public string? VatNumber { get; set; }
    }
}