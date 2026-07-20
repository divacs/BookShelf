using BookShelf.DataAccess.Data;
using BookShelf.Models.Models;
using BookShelf.Utility;
using BookShelfWeb.Configuration;
using BookShelfWeb.MockData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookShelfWeb.Services
{
    public class DevelopmentMockDataSeeder
    {
        private const string AdminEmail = "admin@bookshelf.local";
        private const string CustomerEmail = "mila.cole@bookshelf.local";
        private const string DefaultPassword = "Portfolio123!";

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        private readonly MockStorefrontOptions _options;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public DevelopmentMockDataSeeder(
            ApplicationDbContext db,
            IWebHostEnvironment environment,
            IOptions<MockStorefrontOptions> options,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _environment = environment;
            _options = options.Value;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (!_environment.IsDevelopment() || !_options.Enabled)
            {
                return;
            }

            await _db.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedCatalogAsync();
            await SeedUsersAsync();
            await SeedCustomerDataAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = [SD.Role_Admin, SD.Role_Customer, SD.Role_Company, SD.Role_Employee];

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedCatalogAsync()
        {
            var currentIsbns = await _db.Products.AsNoTracking().Select(product => product.ISBN).ToListAsync();
            var expectedIsbns = BookMockCatalog.Books.Select(book => book.ISBN).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var needsRefresh = currentIsbns.Count != expectedIsbns.Count || currentIsbns.Any(isbn => !expectedIsbns.Contains(isbn));

            if (!needsRefresh)
            {
                return;
            }

            _db.OrderDetails.RemoveRange(_db.OrderDetails);
            _db.OrderHeaders.RemoveRange(_db.OrderHeaders);
            _db.ShoppingCarts.RemoveRange(_db.ShoppingCarts);
            _db.Products.RemoveRange(_db.Products);
            _db.Categories.RemoveRange(_db.Categories);
            _db.Companies.RemoveRange(_db.Companies);
            await _db.SaveChangesAsync();

            var categories = BookMockCatalog.Books
                .Select(book => book.CategoryName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select((name, index) => new Category
                {
                    Name = name,
                    DisplayOrder = index + 1
                })
                .ToList();

            await _db.Categories.AddRangeAsync(categories);
            await _db.SaveChangesAsync();

            await _db.Companies.AddRangeAsync(
            [
                new Company { Name = "Northwind Book Distribution", StreetAddress = "415 Harbor Avenue", City = "Seattle", State = "WA", PostalCode = "98104", PhoneNumber = "(206) 555-0131" },
                new Company { Name = "Maple Street Retail Group", StreetAddress = "22 Irving Square", City = "Boston", State = "MA", PostalCode = "02108", PhoneNumber = "(617) 555-0142" },
                new Company { Name = "Blue Shelf Campus Stores", StreetAddress = "78 College Road", City = "Austin", State = "TX", PostalCode = "73301", PhoneNumber = "(512) 555-0174" },
                new Company { Name = "Paper Lantern Wholesale", StreetAddress = "900 Market Street", City = "San Francisco", State = "CA", PostalCode = "94103", PhoneNumber = "(415) 555-0198" }
            ]);
            await _db.SaveChangesAsync();

            var categoryMap = await _db.Categories.ToDictionaryAsync(category => category.Name, StringComparer.OrdinalIgnoreCase);
            var products = BookMockCatalog.Books.Select(book => new Product
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                ISBN = book.ISBN,
                ListPrice = book.ListPrice,
                Price = book.Price,
                Price50 = book.Price50,
                Price100 = book.Price100,
                CategoryId = categoryMap[book.CategoryName].Id,
                ImageUrl = book.ImageUrl
            });

            await _db.Products.AddRangeAsync(products);
            await _db.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            await EnsureUserAsync(AdminEmail, "Portfolio Admin", SD.Role_Admin, "(555) 019-1024", "18 Market Lane", "Chicago", "IL", "60601");
            await EnsureUserAsync(CustomerEmail, "Mila Cole", SD.Role_Customer, "(555) 012-6621", "42 Orchard Street", "Brooklyn", "NY", "11211");
        }

        private async Task EnsureUserAsync(string email, string name, string role, string phoneNumber, string streetAddress, string city, string state, string postalCode)
        {
            var existingUser = await _db.ApplicationUsers.FirstOrDefaultAsync(user => user.Email == email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Name = name,
                    PhoneNumber = phoneNumber,
                    StreetAddress = streetAddress,
                    City = city,
                    State = state,
                    PostalCode = postalCode
                };

                var createResult = await _userManager.CreateAsync(user, DefaultPassword);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Mock user '{email}' could not be created: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
                }
            }
            else
            {
                existingUser.Name = name;
                existingUser.PhoneNumber = phoneNumber;
                existingUser.StreetAddress = streetAddress;
                existingUser.City = city;
                existingUser.State = state;
                existingUser.PostalCode = postalCode;
                existingUser.EmailConfirmed = true;
                _db.ApplicationUsers.Update(existingUser);
                await _db.SaveChangesAsync();
            }

            var roleUser = await _userManager.FindByEmailAsync(email);
            if (roleUser != null && !await _userManager.IsInRoleAsync(roleUser, role))
            {
                await _userManager.AddToRoleAsync(roleUser, role);
            }
        }

        private async Task SeedCustomerDataAsync()
        {
            var customer = await _db.ApplicationUsers.FirstOrDefaultAsync(user => user.Email == CustomerEmail);
            if (customer == null)
            {
                return;
            }

            var customerOrderIds = await _db.OrderHeaders
                .Where(order => order.ApplicationUserId == customer.Id)
                .Select(order => order.Id)
                .ToListAsync();

            _db.OrderDetails.RemoveRange(_db.OrderDetails.Where(detail => customerOrderIds.Contains(detail.OrderHeaderId)));
            _db.OrderHeaders.RemoveRange(_db.OrderHeaders.Where(order => customerOrderIds.Contains(order.Id)));
            _db.ShoppingCarts.RemoveRange(_db.ShoppingCarts.Where(cart => cart.ApplicationUserId == customer.Id));
            await _db.SaveChangesAsync();

            var products = await _db.Products.ToDictionaryAsync(product => product.ISBN, StringComparer.OrdinalIgnoreCase);

            await _db.ShoppingCarts.AddRangeAsync(BookMockCatalog.CartIsbnsWithCounts.Select(pair => new ShoppingCart
            {
                ApplicationUserId = customer.Id,
                ProductId = products[pair.Key].Id,
                Count = pair.Value
            }));

            var orders = new[]
            {
                CreateOrderHeader(customer, new DateTime(2026, 5, 14), new DateTime(2026, 5, 17), "Delivered", SD.PaymentStatusApproved, "TRK-814209", 77.96),
                CreateOrderHeader(customer, new DateTime(2026, 6, 28), new DateTime(2026, 7, 1), SD.StatusShipped, SD.PaymentStatusApproved, "TRK-926430", 49.98),
                CreateOrderHeader(customer, new DateTime(2026, 7, 16), new DateTime(2026, 7, 18), SD.StatusInProcess, SD.PaymentStatusPending, "TRK-941125", 32.99)
            };

            await _db.OrderHeaders.AddRangeAsync(orders);
            await _db.SaveChangesAsync();

            await _db.OrderDetails.AddRangeAsync(
            [
                new OrderDetail { OrderHeaderId = orders[0].Id, ProductId = products["9781736102102"].Id, Count = 1, Price = 24.99 },
                new OrderDetail { OrderHeaderId = orders[0].Id, ProductId = products["9781736102113"].Id, Count = 2, Price = 15.99 },
                new OrderDetail { OrderHeaderId = orders[1].Id, ProductId = products["9781736102105"].Id, Count = 1, Price = 23.99 },
                new OrderDetail { OrderHeaderId = orders[1].Id, ProductId = products["9781736102116"].Id, Count = 1, Price = 25.99 },
                new OrderDetail { OrderHeaderId = orders[2].Id, ProductId = products["9781736102109"].Id, Count = 1, Price = 32.99 }
            ]);
            await _db.SaveChangesAsync();
        }

        private static OrderHeader CreateOrderHeader(ApplicationUser customer, DateTime orderDate, DateTime shippingDate, string status, string paymentStatus, string trackingNumber, double total) =>
            new()
            {
                ApplicationUserId = customer.Id,
                OrderDate = orderDate,
                ShippingDate = shippingDate,
                PaymentDate = orderDate,
                PaymentDueDate = DateOnly.FromDateTime(orderDate.AddDays(14)),
                OrderStatus = status,
                PaymentStatus = paymentStatus,
                TrackingNumber = trackingNumber,
                Carrier = "BookShelf Logistics",
                OrderTotal = total,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber ?? string.Empty,
                StreetAddress = customer.StreetAddress ?? string.Empty,
                City = customer.City ?? string.Empty,
                State = customer.State ?? string.Empty,
                PostalCode = customer.PostalCode ?? string.Empty
            };
    }
}
