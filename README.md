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


# Developer Questions

## Contents

1. [What is the difference between interfaces and abstract classes? Provide an example where you would use each.](#1-what-is-the-difference-between-interfaces-and-abstract-classes-provide-an-example-where-you-would-use-each)
2. [What are the advantages of using interfaces?](#2-what-are-the-advantages-of-using-interfaces)
3. [Give an example of inheritance i.e., real life business example where it could be used?](#3-give-an-example-of-inheritance-ie-real-life-business-example-where-it-could-be-used)

---

## 1. What is the difference between interfaces and abstract classes? Provide an example where you would use each.

Both interfaces and abstract classes are used for abstraction in OOP, but they solve slightly different problems. The way I think about it is that an interface says what a class can do, while an abstract class says what a class is. An interface is basically a contract: any class that implements it has to provide those members. An abstract class can't be instantiated on its own. It's meant to be a base class that related classes inherit from, so they can share code and state.

The main differences:

| Feature | Interface | Abstract class |
| --- | --- | --- |
| Purpose | Defines what a type can do | Provides a shared base for related classes |
| Inheritance | A class can implement multiple interfaces | A class can inherit from only one class |
| Constructors | Cannot have instance constructors | Can have constructors |
| Methods | Can define contracts and, in modern C#, default implementations | Can contain abstract and concrete methods |
| Fields and state | Cannot have instance fields | Can store instance state |
| Coupling | Encourages loose coupling | Useful for sharing common implementation |
| Best used when | Unrelated classes need the same capability | Related classes share common behaviour or state |

### Interface example

A payment system is a good example. The checkout process needs to take payments, but it shouldn't care which provider is actually doing the work.

*Step 1: Create the interface*

```csharp
public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount);
}
```

*Step 2: Implement the interface*

```csharp
public class PayPalProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine("Processing payment through PayPal...");
        return true;
    }
}

public class StripeProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine("Processing payment through Stripe...");
        return true;
    }
}
```

*Step 3: Use the interface*

```csharp
public class Checkout
{
    private readonly IPaymentProcessor _processor;

    public Checkout(IPaymentProcessor processor)
    {
        _processor = processor;
    }

    public void CompleteOrder(decimal amount)
    {
        bool success = _processor.ProcessPayment(amount);

        if (success)
            Console.WriteLine("Payment successful!");
    }
}
```

> [!NOTE]
> **Why an interface works here:** `Checkout` only depends on `IPaymentProcessor`, not on PayPal or Stripe directly. If the business wants to add another provider later, I just write a new class that implements the interface and nothing in `Checkout` has to change. It also makes `Checkout` easy to unit test, because I can pass in a fake processor instead of calling a real payment gateway.

### Abstract class example

Employees are a good fit for an abstract class. Every employee has a name and can display it, but how their pay is worked out depends on the type of employee. It also doesn't make sense to create a plain "Employee" that isn't one of the specific types.

*Step 1: Create the abstract class*

```csharp
public abstract class Employee
{
    public string Name { get; set; }

    protected Employee(string name)
    {
        Name = name;
    }

    public void DisplayName()
    {
        Console.WriteLine($"Employee: {Name}");
    }

    public abstract decimal CalculateSalary();
}
```

*Step 2: Inherit from the abstract class*

```csharp
public class FullTimeEmployee : Employee
{
    public FullTimeEmployee(string name) : base(name) { }

    public override decimal CalculateSalary()
    {
        return 30000m;
    }
}

public class PartTimeEmployee : Employee
{
    public PartTimeEmployee(string name) : base(name) { }

    public override decimal CalculateSalary()
    {
        return 8000m;
    }
}
```

*Step 3: Use the classes*

```csharp
Employee employee = new FullTimeEmployee("John");
employee.DisplayName();
Console.WriteLine(employee.CalculateSalary());
```

> [!NOTE]
> **Why an abstract class works here:** The shared parts (the name and `DisplayName()`) are written once in the base class. `CalculateSalary()` is marked abstract, so every employee type is forced to provide its own version, and the compiler will complain if one is missed.
---

## 2. What are the advantages of using interfaces?

- **Loose coupling:** code depends on the contract rather than a specific class, so I can change or replace an implementation without breaking the code that uses it.
- **Abstraction:** the caller only needs to know what a class does, not how it does it.
- **Polymorphism:** different classes can be treated the same way as long as they implement the same interface, like the PayPal and Stripe processors above.
- **Multiple interfaces:** C# only allows a class to inherit from one base class, but it can implement as many interfaces as it needs.
- **Testing and dependency injection:** it's easy to swap in a different implementation or a mock object, which makes unit testing a lot simpler. ASP.NET Core's built-in DI relies heavily on this.
- **Working in a team:** once the interface is agreed on, different developers can build each side at the same time without waiting on each other.

---

## 3. Give an example of inheritance i.e., real life business example where it could be used?

Inheritance lets one class take on the properties and methods of another, so common code is written once in a base class and reused by the classes that inherit from it.

An example from a business setting would be an HR or payroll system. Every employee has a name and an employee ID no matter what kind of employee they are, but full-time staff are paid a monthly salary and part-time staff are paid per hour. So I'd put the common details in an `Employee` base class and have `FullTimeEmployee` and `PartTimeEmployee` inherit from it, each adding only what's specific to them.

```csharp
public class Employee
{
    public string Name { get; set; }
    public int EmployeeId { get; set; }

    public void DisplayDetails()
    {
        Console.WriteLine($"{Name} - {EmployeeId}");
    }
}

public class FullTimeEmployee : Employee
{
    public decimal MonthlySalary { get; set; }
}

public class PartTimeEmployee : Employee
{
    public decimal HourlyRate { get; set; }
}
```

### Why inheritance helps here

- **Code reuse:** both classes get `Name`, `EmployeeId` and `DisplayDetails()` from `Employee` without rewriting them.
- **Less duplication:** the shared properties only live in one place.
- **Easier to maintain:** if the business wants to add something to every employee, like a start date, I only need to change the base class.
