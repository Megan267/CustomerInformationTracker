namespace CustomerTrackerMVC.Models
{
    public class CustomerPage
    {
        public List<Customer> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; } = 1;
    }
}
