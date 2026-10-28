namespace CustomerTrackerMVC.Models
{
    /// <summary>
    /// The columns the customer list can be sorted by.
    /// Each name matches a "sortBy" value the API understands.
    /// </summary>
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

    /// <summary>
    /// What the customer list page reads from its query string,
    /// e.g. /Customers?name=acme&amp;vat=401&amp;sort=City&amp;desc=true&amp;page=2
    /// </summary>
    public class CustomerListQuery
    {
        public string? Name { get; set; }

        public string? Vat { get; set; }

        public CustomerSortField Sort { get; set; } = CustomerSortField.Name;

        public bool Desc { get; set; }

        public int Page { get; set; } = 1;

        public bool HasFilter => !string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(Vat);

        /// <summary>
        /// Builds the query string for GET /api/customer.
        /// The API uses different parameter names (VatNumber, SortBy, SortDir) to this page.
        /// </summary>
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

            return string.Join("&", parts);
        }
    }
}
