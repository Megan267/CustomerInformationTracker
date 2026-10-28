using CustomerTrackerAPI.Data;
using CustomerTrackerAPI.DTOs;
using CustomerTrackerAPI.Mappings;
using CustomerTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            if (query.IncludeDeleted)
            {
                customers = customers.IgnoreQueryFilters();
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

        /// <summary>
        /// Reinstates a deleted customer by setting its IsActive property to true.
        /// </summary>
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
                "street" => desc ? customers.OrderByDescending(c => c.Address!.Street) : customers.OrderBy(c => c.Address!.Street),
                "city" => desc ? customers.OrderByDescending(c => c.Address!.City) : customers.OrderBy(c => c.Address!.City),
                "province" => desc ? customers.OrderByDescending(c => c.Address!.Province) : customers.OrderBy(c => c.Address!.Province),
                "postalcode" => desc ? customers.OrderByDescending(c => c.Address!.PostalCode) : customers.OrderBy(c => c.Address!.PostalCode),
                "country" => desc ? customers.OrderByDescending(c => c.Address!.Country) : customers.OrderBy(c => c.Address!.Country),
                "telephonenumber" => desc ? customers.OrderByDescending(c => c.TelephoneNumber) : customers.OrderBy(c => c.TelephoneNumber),
                "contactpersonname" => desc ? customers.OrderByDescending(c => c.ContactPersonName) : customers.OrderBy(c => c.ContactPersonName),
                "contactpersonemail" => desc ? customers.OrderByDescending(c => c.ContactPersonEmail) : customers.OrderBy(c => c.ContactPersonEmail),
                "vatnumber" => desc ? customers.OrderByDescending(c => c.VatNumber) : customers.OrderBy(c => c.VatNumber),
                "createdat" => desc ? customers.OrderByDescending(c => c.CreatedAt) : customers.OrderBy(c => c.CreatedAt),
                _ => desc ? customers.OrderByDescending(c => c.Name) : customers.OrderBy(c => c.Name),
            };

            return sorted.ThenBy(c => c.CustomerId);
        }
    }
}