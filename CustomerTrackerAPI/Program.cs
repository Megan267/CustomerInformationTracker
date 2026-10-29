using CustomerTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using System.Text.Json.Serialization;

namespace CustomerTrackerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddOpenApi();

            // Locally this comes from User Secrets; on Azure from the
            // ConnectionStrings__DefaultConnection app setting (or the Connection strings tab).
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection is not set. Add it to User Secrets locally, " +
                    "or as ConnectionStrings__DefaultConnection in the App Service's environment variables on Azure.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Customer Tracker API v1");
                    options.RoutePrefix = "swagger";
                });

                if (!SetupDatabase(app))
                {
                    return;
                }
            }
            else
            {
                // On Azure (Production) only apply migrations that already exist in the project.
                // New migrations are never created here. This is for the demo only -> a production database would not be seeded.
                ApplyMigrations(app);
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static bool SetupDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            bool hasMigrations = db.Database.GetMigrations().Any();

            if (!hasMigrations || db.Database.HasPendingModelChanges())
            {
                string name = hasMigrations
                    ? "Migration_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                    : "InitialSetup";

                CreateMigration(db, name, app.Environment.ContentRootPath);

                Console.WriteLine($"Created migration {name}. Rebuild and run again to apply it.");
                return false;
            }

            var pending = db.Database.GetPendingMigrations().ToList();
            if (pending.Any())
            {
                Console.WriteLine("Applying: " + string.Join(", ", pending));
                db.Database.Migrate();
            }

            return true;
        }

        private static void ApplyMigrations(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var pending = db.Database.GetPendingMigrations().ToList();
            if (pending.Any())
            {
                app.Logger.LogInformation("Applying migrations: {Migrations}", string.Join(", ", pending));
                db.Database.Migrate();
            }
        }

        private static void CreateMigration(ApplicationDbContext db, string name, string projectPath)
        {
            var services = new ServiceCollection()
                .AddEntityFrameworkDesignTimeServices()
                .AddDbContextDesignTimeServices(db);
#pragma warning disable EF1001
            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(services);
#pragma warning restore EF1001

            var scaffolder = services.BuildServiceProvider()
                .GetRequiredService<IMigrationsScaffolder>();

            var migration = scaffolder.ScaffoldMigration(name, "CustomerTrackerAPI", "Migrations");
            scaffolder.Save(projectPath, migration, "Migrations");
        }
    }
}