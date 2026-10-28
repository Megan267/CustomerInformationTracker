namespace CustomerTrackerAPI.DTOs
{
    /// <summary>What the API sends back for a customer.</summary>
    public record CustomerResponse(
        int CustomerId,
        string Name,
        AddressResponse? Address,
        string? TelephoneNumber,
        string? ContactPersonName,
        string? ContactPersonEmail,
        string? VatNumber,
        bool IsActive,
        DateTime CreatedAt);

    public record AddressResponse(
        int AddressId,
        string Street,
        string City,
        string? Province,
        string PostalCode,
        string Country);
}