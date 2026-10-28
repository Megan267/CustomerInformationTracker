namespace CustomerTrackerAPI.DTOs
{
    public class CustomerQuery
    {
        public string? Name { get; set; }

        public string? VatNumber { get; set; }

        public string? SortBy { get; set; }

        public string? SortDir { get; set; }

        public int Page { get; set; } = 1;

        public bool IncludeDeleted { get; set; }

        public string? Status { get; set; }
    }
}