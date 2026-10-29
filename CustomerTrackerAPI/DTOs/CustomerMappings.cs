using CustomerTrackerAPI.DTOs;
using CustomerTrackerAPI.Models;

namespace CustomerTrackerAPI.Mappings
{
    public static class CustomerMappings
    {
        public static Customer ToEntity(this CustomerRequest request)
        {
            return new Customer
            {
                Name = request.Name.Trim(),
                Address = request.Address.ToEntity(),
                TelephoneNumber = Clean(request.TelephoneNumber),
                ContactPersonName = Clean(request.ContactPersonName),
                ContactPersonEmail = Clean(request.ContactPersonEmail),
                VatNumber = Clean(request.VatNumber)
            };
        }

        public static Address ToEntity(this AddressRequest request)
        {
            return new Address
            {
                Street = request.Street.Trim(),
                City = request.City.Trim(),
                Province = Clean(request.Province),
                PostalCode = request.PostalCode.Trim(),
                Country = request.Country.Trim()
            };
        }

        public static CustomerResponse ToResponse(this Customer customer)
        {
            return new CustomerResponse(
                customer.CustomerId,
                customer.Name,
                customer.Address?.ToResponse(),
                customer.TelephoneNumber,
                customer.ContactPersonName,
                customer.ContactPersonEmail,
                customer.VatNumber,
                customer.IsActive,
                customer.CreatedAt);
        }

        public static AddressResponse ToResponse(this Address address)
        {
            return new AddressResponse(
                address.AddressId,
                address.Street,
                address.City,
                address.Province,
                address.PostalCode,
                address.Country);
        }

        private static string? Clean(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static void UpdateFrom(this Customer customer, CustomerRequest request)
        {
            customer.Name = request.Name.Trim();
            customer.TelephoneNumber = Clean(request.TelephoneNumber);
            customer.ContactPersonName = Clean(request.ContactPersonName);
            customer.ContactPersonEmail = Clean(request.ContactPersonEmail);
            customer.VatNumber = Clean(request.VatNumber);

            if (customer.Address == null)
            {
                customer.Address = request.Address.ToEntity();
            }
            else
            {
                customer.Address.Street = request.Address.Street.Trim();
                customer.Address.City = request.Address.City.Trim();
                customer.Address.Province = Clean(request.Address.Province);
                customer.Address.PostalCode = request.Address.PostalCode.Trim();
                customer.Address.Country = request.Address.Country.Trim();
            }
        }
    }
}