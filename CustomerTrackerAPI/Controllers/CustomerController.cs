using CustomerTrackerAPI.Data;
using CustomerTrackerAPI.DTOs;
using CustomerTrackerAPI.Mappings;
using CustomerTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private const int PageSize = 10;

        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<CustomerResponse>>> GetCustomers([FromQuery] CustomerQuery query)
        {
            IQueryable<Customer> customers = _context.Customers
                .AsNoTracking()
                .Include(c => c.Address);

            string status = query.Status?.Trim().ToLowerInvariant() ?? "active";

            if (query.IncludeDeleted || status is "inactive" or "all")
            {
                customers = customers.IgnoreQueryFilters();
            }

            if (status == "inactive")
            {
                customers = customers.Where(c => !c.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                string name = query.Name.Trim();
                customers = customers.Where(c => c.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(query.VatNumber))
            {
                string vat = query.VatNumber.Trim();
                customers = customers.Where(c => c.VatNumber != null && c.VatNumber.Contains(vat));
            }

            int totalCount = await customers.CountAsync();
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
            int page = Math.Clamp(query.Page, 1, totalPages);

            var items = await ApplySort(customers, query.SortBy, query.SortDir)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Ok(new PagedResponse<CustomerResponse>(
                items.Select(c => c.ToResponse()).ToList(),
                totalCount,
                page,
                PageSize,
                totalPages));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer.ToResponse());
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomer([FromBody] CustomerRequest request)
        {
            var customer = request.ToEntity();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer.ToResponse());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer(int id, [FromBody] CustomerRequest request)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.UpdateFrom(request);
            await _context.SaveChangesAsync();

            return Ok(customer.ToResponse());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer is null)
            {
                return NotFound();
            }

            customer.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id:int}/reinstate")]
        public async Task<IActionResult> ReinstateCustomer(int id)
        {
            var customer = await _context.Customers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer is null)
            {
                return NotFound();
            }

            customer.IsActive = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static IQueryable<Customer> ApplySort(IQueryable<Customer> customers, string? sortBy, string? sortDir)
        {
            bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            IOrderedQueryable<Customer> sorted = (sortBy?.Trim().ToLowerInvariant()) switch
            {
                "street" => By(customers, c => c.Address!.Street, desc),
                "city" => desc
                    ? By(customers, c => c.Address!.City, desc).ThenByDescending(c => c.Address!.Street)
                    : By(customers, c => c.Address!.City, desc).ThenBy(c => c.Address!.Street),
                "province" => NullsLast(customers, c => c.Address!.Province, desc),
                "postalcode" => By(customers, c => c.Address!.PostalCode, desc),
                "country" => By(customers, c => c.Address!.Country, desc),
                "telephonenumber" => NullsLast(customers, c => c.TelephoneNumber == null ? null : c.TelephoneNumber
                    .Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+27", "0"), desc),
                "contactpersonname" => NullsLast(customers, c => c.ContactPersonName, desc),
                "contactpersonemail" => NullsLast(customers, c => c.ContactPersonEmail, desc),
                "vatnumber" => NullsLast(customers, c => c.VatNumber, desc),
                "createdat" => By(customers, c => c.CreatedAt, desc),
                _ => By(customers, c => c.Name, desc),
            };

            return sorted.ThenBy(c => c.CustomerId);
        }

        private static IOrderedQueryable<Customer> By<TKey>(
            IQueryable<Customer> customers, Expression<Func<Customer, TKey>> key, bool desc)
        {
            return desc ? customers.OrderByDescending(key) : customers.OrderBy(key);
        }

        private static IOrderedQueryable<Customer> NullsLast(
            IQueryable<Customer> customers, Expression<Func<Customer, string?>> key, bool desc)
        {
            var isNull = Expression.Lambda<Func<Customer, bool>>(
                Expression.Equal(key.Body, Expression.Constant(null, typeof(string))),
                key.Parameters);

            var withValuesFirst = customers.OrderBy(isNull);
            return desc ? withValuesFirst.ThenByDescending(key) : withValuesFirst.ThenBy(key);
        }
    }
}