namespace CustomerTrackerMVC.Models
{
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
            $"{Result.TotalCount} {(Query.Status == CustomerStatusFilter.Inactive ? "inactive " : "")}" +
            $"{(Result.TotalCount == 1 ? "customer" : "customers")}{(Query.HasSearch ? (Result.TotalCount == 1 ? " matches" : " match") : "")}";
    }
}