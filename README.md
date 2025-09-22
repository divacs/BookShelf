# 📚 BookShelf

**BookShelf** is a modern **.NET 8 MVC web application** for **buying and
managing books**.\
It was developed as a **showcase project** with emphasis on **clean
architecture, scalability, and real-world e-commerce practices**.

------------------------------------------------------------------------

## 🚀 Technologies & Architecture

-   **.NET 8 MVC** with Razor Pages\
-   **N-Tier Architecture** (UI, Application, Infrastructure, Domain)\
-   **Repository Pattern + Unit of Work**\
-   **Entity Framework Core** (Code First + Migrations)\
-   **ASP.NET Identity** (scaffolded & customized)\
-   **Role-based Authorization** (Admin, Manager, Customer)\
-   **Facebook Single Sign-On (SSO)**\
-   **ViewData / ViewBag / TempData**\
-   **Session Management** in .NET Core\
-   **View Components** for reusability\
-   **SweetAlert integration** for better UX

📌 Note: Stripe (payments) and SendGrid (emails) were not implemented
due to service restrictions in Serbia, but their integration flow and
configuration in .NET were studied.\
The same applies for **Azure App Service deployment** --- process was
fully researched.

------------------------------------------------------------------------

## 🎯 Features

-   ✅ Book purchase flow\
-   ✅ Role-based user management (Admin Panel)\
-   ✅ Facebook login (SSO)\
-   ✅ SweetAlert notifications and validations

------------------------------------------------------------------------

## 🏗️ Project Structure

    BookShelf/
    │-- BookShelf.Web (UI Layer - MVC + Razor Pages + Areas)
    │   │-- Areas/
    │   │   │-- Admin (Controllers + Views: Category, Company, Product)
    │   │   │-- Customer (Controllers + Views: Cart, Home)
    │   │   │-- Identity (Account management, Identity scaffolding)
    │   │   │-- Shared (Layouts, partial views, components)
    │
    │-- BookShelf.DataAccess (EF Core, Repositories, UnitOfWork, DbContext, Migrations)
    │-- BookShelf.Models (Entities, ViewModels)
    │-- BookShelf.Utility (Helper classes: EmailSender, constants, extensions)

------------------------------------------------------------------------

## ⚡ Installation & Setup

1.  Clone the repository\

``` bash
git clone https://github.com/divacs/BookShelf.git
cd BookShelf
```

2.  Apply migrations and create the database\

``` bash
dotnet ef database update
```

3.  Run the application\

``` bash
dotnet run --project BookShelf.Web
```

4.  Open in browser\

```{=html}
<!-- -->
```
    https://localhost:5001

------------------------------------------------------------------------

## 🔑 Roles & Login

-   **Admin** -- full access (books, users, orders)\
-   **Manager** -- access to reports and orders\
-   **Customer** -- purchase and account features


------------------------------------------------------------------------

## ☁️ Deployment & Integrations

-   **Azure App Service deployment** -- studied\
-   **Stripe integration** -- studied (payments, refunds)\
-   **SendGrid integration** -- studied (email notifications)

------------------------------------------------------------------------

## 📬 Contact

👩‍💻 Author: Sonja Divac\
📧 Email: sonja.divac@yahoo.com

------------------------------------------------------------------------

✨ **BookShelf** demonstrates **mid-level .NET developer skills**,
focusing on **architecture, best practices, and practical application of
modern .NET features**.
