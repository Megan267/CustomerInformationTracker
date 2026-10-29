using System.ComponentModel.DataAnnotations;

namespace CustomerTrackerAPI.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter in the customer's name.")]
        [StringLength(200, ErrorMessage = "The name can be at most {1} characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter in the customer's address.")]
        [Display(Name = "Address")]
        public Address? Address { get; set; }

        [StringLength(20, ErrorMessage = "The telephone number can be at most {1} characters.")]
        [RegularExpression(@"^\+?(?:[\s\-()]{0,2}\d){7,15}$",
            ErrorMessage = "Enter a valid phone number, e.g. 021 123 4567.")]
        [Display(Name = "Telephone number")]
        public string? TelephoneNumber { get; set; }

        [StringLength(100, ErrorMessage = "The contact person's name can be at most {1} characters.")]
        [Display(Name = "Contact person")]
        public string? ContactPersonName { get; set; }

        [StringLength(100, ErrorMessage = "The email address can be at most {1} characters.")]
        [RegularExpression(@"^[A-Za-z0-9_%+\-]+(?:\.[A-Za-z0-9_%+\-]+)*@[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?)*\.[A-Za-z]{2,}$",
            ErrorMessage = "Enter a valid email address, e.g. name@gmail.com")]
        [Display(Name = "Contact email")]
        public string? ContactPersonEmail { get; set; }

        [StringLength(20, ErrorMessage = "The VAT number can be at most {1} characters.")]
        [RegularExpression(@"^(?:[A-Za-z]{2} ?)?\d(?:[ .\-]?\d){7,17}$",
        ErrorMessage = "Enter a valid VAT number, e.g. 4123456789 or GB123456789.")]
        [Display(Name = "VAT number")]
        public string? VatNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}