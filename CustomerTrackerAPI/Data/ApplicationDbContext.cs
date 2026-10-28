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

        //Configures the model and relationships between entities in the database context.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Address>().ToTable("Addresses");

            // Customer -> Address (an address can have many customers, but a customer only has one address)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.address)
                .WithMany()
                .HasForeignKey(c => c.addressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Emails must be unique per customer
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.email)
                .IsUnique();
        }
    }
}
// dotnet ef migrations add InitialSetup
// dotnet ef database update