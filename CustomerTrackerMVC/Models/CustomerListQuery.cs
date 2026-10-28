namespace CustomerTrackerMVC.Models
{
    public enum CustomerSortField
    {
        Name,
        City,
        TelephoneNumber,
        ContactPersonName,
        ContactPersonEmail,
        VatNumber,
        CreatedAt
    }

    public enum CustomerStatusFilter
    {
        Active,
        Inactive,
        All
    }

    public class CustomerListQuery
    {
        public string? Name { get; set; }

        public string? Vat { get; set; }

        public CustomerSortField Sort { get; set; } = CustomerSortField.Name;

        public bool Desc { get; set; }

        public int Page { get; set; } = 1;

        public CustomerStatusFilter Status { get; set; } = CustomerStatusFilter.Active;

        public bool HasSearch => !string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(Vat);

        public bool HasFilter => HasSearch || Status != CustomerStatusFilter.Active;

        public string? StatusRouteValue => Status == CustomerStatusFilter.Active ? null : Status.ToString();

        
        public string ToApiQueryString()
        {
            var sort = Enum.IsDefined(Sort) ? Sort : CustomerSortField.Name;

            var parts = new List<string>
            {
                "SortBy=" + sort,
                "SortDir=" + (Desc ? "desc" : "asc"),
                "Page=" + Math.Max(1, Page)
            };

            if (!string.IsNullOrWhiteSpace(Name))
            {
                parts.Add("Name=" + Uri.EscapeDataString(Name.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(Vat))
            {
                parts.Add("VatNumber=" + Uri.EscapeDataString(Vat.Trim()));
            }

            if (Status != CustomerStatusFilter.Active && Enum.IsDefined(Status))
            {
                parts.Add("Status=" + Status.ToString().ToLowerInvariant());
            }

            return string.Join("&", parts);
        }
    }
}