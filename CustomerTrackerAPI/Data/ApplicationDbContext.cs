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

            SeedCustomers(modelBuilder);
        }

        private static void SeedCustomers(ModelBuilder modelBuilder)
        {
            var seededAt = new DateTime(2026, 10, 28, 9, 0, 0);

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, Name = "Acacia Logistics", TelephoneNumber = "011 555 0101", ContactPersonName = "Thandi Mokoena", ContactPersonEmail = "thandi@acacialogistics.co.za", VatNumber = "4012345671", IsActive = true, CreatedAt = seededAt },
                new Customer { CustomerId = 2, Name = "Blue Crane Foods", TelephoneNumber = "021 555 0202", ContactPersonName = "Pieter van der Merwe", ContactPersonEmail = "pieter@bluecranefoods.co.za", VatNumber = "4098765432", IsActive = true, CreatedAt = seededAt },
                new Customer { CustomerId = 3, Name = "Umhlanga Marine Supplies", TelephoneNumber = "031 555 0303", ContactPersonName = "Ayesha Naidoo", ContactPersonEmail = "ayesha@umhlangamarine.co.za", VatNumber = "4111222333", IsActive = true, CreatedAt = seededAt },
                new Customer { CustomerId = 4, Name = "Karoo Solar Installations", TelephoneNumber = "051 555 0404", ContactPersonName = "Lerato Dlamini", ContactPersonEmail = "lerato@karoosolar.co.za", IsActive = true, CreatedAt = seededAt },
                new Customer { CustomerId = 5, Name = "Baobab Print Studio", ContactPersonName = "Sipho Khumalo", ContactPersonEmail = "sipho@baobabprint.co.za", IsActive = true, CreatedAt = seededAt },
                new Customer { CustomerId = 6, Name = "Thames Valley Imports", TelephoneNumber = "+44 20 7946 0958", ContactPersonName = "James Whitfield", ContactPersonEmail = "james@thamesvalleyimports.co.uk", VatNumber = "GB123456789", IsActive = true, CreatedAt = seededAt });

            modelBuilder.Entity<Address>().HasData(
                new Address { AddressId = 1, CustomerId = 1, Street = "14 Wierda Road", City = "Sandton", Province = "Gauteng", PostalCode = "2196", Country = "South Africa" },
                new Address { AddressId = 2, CustomerId = 2, Street = "8 Bree Street", City = "Cape Town", Province = "Western Cape", PostalCode = "8001", Country = "South Africa" },
                new Address { AddressId = 3, CustomerId = 3, Street = "22 Lighthouse Road", City = "Umhlanga", Province = "KwaZulu-Natal", PostalCode = "4319", Country = "South Africa" },
                new Address { AddressId = 4, CustomerId = 4, Street = "3 Nelson Mandela Drive", City = "Bloemfontein", Province = "Free State", PostalCode = "9301", Country = "South Africa" },
                new Address { AddressId = 5, CustomerId = 5, Street = "55 Church Street", City = "Pretoria", Province = "Gauteng", PostalCode = "0002", Country = "South Africa" },
                new Address { AddressId = 6, CustomerId = 6, Street = "10 King Street", City = "London", PostalCode = "EC2V 8EA", Country = "United Kingdom" });
        }
    }
}