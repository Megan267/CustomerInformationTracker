# Trackly

An ASP.NET Core MVC app for capturing and managing customer information, backed by a Web API that uses Entity Framework Core and LINQ against an Azure SQL Database.

## Projects

| Project | What it does |
| --- | --- |
| `CustomerTrackerMVC` | The web front end: the customer list (filter, sort, page, delete) and the add/edit page. It talks to the API over HTTP. |
| `CustomerTrackerAPI` | REST API at `/api/customer`. Owns the database through EF Core (`ApplicationDbContext`) and applies migrations on start-up. |

## Live demo

The app is hosted on Azure App Service (South Africa North):

- **Website:** https://customertracker-f9fmh4aeesfcbzgp.southafricanorth-01.azurewebsites.net
- **API:** https://customertrackerapi-a9ebcjeca4atbfff.southafricanorth-01.azurewebsites.net

## Database

- The data is stored in an **Azure SQL Database**, accessed only by the API through Entity Framework Core and LINQ.
- The schema is created and updated with **EF Core migrations** in `CustomerTrackerAPI/Migrations`, which are applied
  automatically when the API starts.

> [!NOTE]
> The database is **seeded with a few sample customers** (the `SeedCustomers` migration) so the list has data to
> sort, filter and page through straight away. This is for the demo only; a production database would not be seeded.

> [!IMPORTANT]
> The connection string is not stored in the code. On Azure it is set as the `DefaultConnection` connection string on
> the API App Service, and locally it comes from User Secrets. It has this format:
> ```
> Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<database>;User ID=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
> ```

> [!TIP]
> To run it locally instead, start both projects (`CustomerTrackerAPI` and `CustomerTrackerMVC`) in Visual Studio.
> In Development the API uses LocalDB from `appsettings.Development.json` and creates the database on first run.

## How the requirements are met

- **Captured fields:** name, address (street, city, province, postal code, country), telephone number,
  contact person name, contact person email and VAT number.
- **Validation:** name and address are required. If a contact email is given it must be a valid address.
  Telephone and VAT numbers are format-checked. Rules run in the browser, in the MVC model and again in the API,
  and API validation errors are shown against the matching fields.
- **List page:** shows every field, sorts by any column (ascending or descending, with empty values always last),
  pages 10 at a time, and filters by name and VAT number as you type.
- **Delete:** asks for confirmation in a pop-up first. If JavaScript is unavailable, the server shows a separate
  confirmation page instead, so a customer is never deleted without confirming. Deletes are soft (`IsActive = false`),
  so a deleted customer can be restored with *Undo* or from the *Inactive customers* filter.
- **Add / edit page:** one page handles both, with field-level feedback.

## Design notes

- Address is its own table (one-to-one with Customer, cascade delete) so its parts can be validated and sorted separately.
- A global query filter hides inactive customers. Only the status filter and reinstate endpoint bypass it.
- Sorting, filtering and paging happen in SQL through LINQ, so only one page of rows is loaded at a time.
- Indexes on `Name` and `VatNumber` support the two filters.
- The Address column sorts by city, then street, because sorting on the full address would only order by house number.
- The MVC site handles the API being unavailable with a friendly message instead of an error page.
- On phones, the customer table switches to stacked cards so every field and action stays visible.
