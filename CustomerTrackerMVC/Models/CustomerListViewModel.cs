namespace CustomerTrackerMVC.Models
{
    /// <summary>Everything the customer list page and its results partial need.</summary>
    public class CustomerListViewModel
    {
        public CustomerListViewModel(CustomerListQuery query, CustomerPage result)
        {
            Query = query;
            Result = result;
        }

        public CustomerListQuery Query { get; }

        public CustomerPage Result { get; }

        public string CountText =>
            $"{Result.TotalCount} {(Result.TotalCount == 1 ? "customer" : "customers")}{(Query.HasFilter ? " match" : "")}";
    }
}
