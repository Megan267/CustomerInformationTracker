using CustomerTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerTrackerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Address>().ToTable("Addresses");

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)
                .WithOne()
                .HasForeignKey<Address>(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Customer>().HasIndex(c => c.Name);
            modelBuilder.Entity<Customer>().HasIndex(c => c.VatNumber);

            modelBuilder.Entity<Customer>().HasQueryFilter(c => c.IsActive);
        }
    }
}

// dotnet ef migrations add InitialSetup
// dotnet ef database update