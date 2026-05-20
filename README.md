# BookShelf

BookShelf is a .NET 8 ASP.NET Core MVC web application for browsing books,
managing product data, adding books to a shopping cart, and creating basic
orders.

The project is structured as a small portfolio application with separate web,
model, data access, and utility layers.

## Features

- Public customer area for browsing products and viewing product details
- Shopping cart for authenticated users
- Basic order summary and order creation flow
- Admin area for managing categories, products, and companies
- ASP.NET Identity registration, login, roles, and account pages
- Role-based access for the Admin area
- Product image upload for admin product management
- EF Core migrations and seed data for categories, companies, and sample products
- Session-based cart count support
- Development/mock email sender for Identity email flows

## Tech Stack

- .NET 8
- ASP.NET Core MVC
- Razor Pages for ASP.NET Identity
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Repository Pattern and Unit of Work
- Bootstrap
- jQuery validation
- Toastr notifications

## Project Structure

```text
BookShelf/
|-- BookShelfWeb/          Web app, MVC controllers, Razor views, Identity pages
|-- BookShelf.DataAccess/  EF Core DbContext, repositories, UnitOfWork, migrations
|-- BookShelf.Models/      Entity models and view models
|-- BookShelf.Utility/     Shared constants and development email sender
|-- BookShelf.sln          Visual Studio solution
```

Main web areas:

```text
BookShelfWeb/Areas/Admin     Category, Product, and Company management
BookShelfWeb/Areas/Customer  Home, product details, cart, and order summary
BookShelfWeb/Areas/Identity  Scaffolded ASP.NET Identity pages
```

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- SQL Server or SQL Server Express
- Entity Framework Core CLI tools

If EF tools are not installed:

```bash
dotnet tool install --global dotnet-ef
```

### Restore Packages

From the repository root:

```bash
dotnet restore
```

### Configure the Database

Update the `DefaultConnection` connection string in:

```text
BookShelfWeb/appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=BookShelfDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### Apply Migrations

From the repository root:

```bash
dotnet ef database update --project BookShelf.DataAccess --startup-project BookShelfWeb
```

### Run the App

```bash
dotnet run --project BookShelfWeb
```

Then open the HTTPS URL shown in the terminal.

## Roles

The application defines these role constants:

- Admin
- Customer
- Company
- Employee

Admin controllers are restricted to users in the `Admin` role. Roles are created
from the registration page code path if they do not already exist.

## Notes and Limitations

- Email sending is implemented as a development/mock `EmailSender` that writes
  email details to the console. It is not a production email integration.
- Stripe payments are not implemented.
- Facebook login/SSO is not configured in the current codebase.
- There is no deployment pipeline or cloud hosting configuration in this repo.
- The order flow creates local order records, but does not process real payments.

## Future Improvements

- Add production email provider configuration
- Add payment provider integration
- Add tests for cart, order, and admin workflows
- Improve seed/admin setup documentation
- Add deployment documentation after deployment is actually configured

