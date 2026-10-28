using System.ComponentModel.DataAnnotations;

namespace CustomerTrackerAPI.DTOs
{
    public class AddressRequest
    {
        [Required(ErrorMessage = "Please enter the street address.")]
        [StringLength(200, ErrorMessage = "The street name can be at most {1} characters.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the city's name.")]
        [StringLength(100, ErrorMessage = "The city's name can be at most {1} characters.")]
        public string City { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "The province name can be at most {1} characters.")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Please enter the postal code.")]
        [StringLength(20, ErrorMessage = "The postal code can be at most {1} characters.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the country.")]
        [StringLength(100, ErrorMessage = "The country name can be at most {1} characters.")]
        public string Country { get; set; } = string.Empty;
    }
}