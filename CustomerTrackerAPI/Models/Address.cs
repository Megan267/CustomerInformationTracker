using System.ComponentModel.DataAnnotations;

namespace CustomerTrackerAPI.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter the street address.")]
        [StringLength(200, ErrorMessage = "The street name can be at most {1} characters.")]
        [Display(Name = "Street")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the city's name.")]
        [StringLength(100, ErrorMessage = "The city's name can be at most {1} characters.")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "The province name can be at most {1} characters.")]
        [Display(Name = "Province")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Please enter the postal code.")]
        [StringLength(20, ErrorMessage = "The postal code can be at most {1} characters.")]
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the country.")]
        [StringLength(100, ErrorMessage = "The country name can be at most {1} characters.")]
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;
    }
}