using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Utility;
using BookShelfWeb.Services;
using BookShelfWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShelfWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMockStorefrontService _mockStorefrontService;

        public DashboardController(IUnitOfWork unitOfWork, IMockStorefrontService mockStorefrontService)
        {
            _unitOfWork = unitOfWork;
            _mockStorefrontService = mockStorefrontService;
        }

        public IActionResult Index()
        {
            var allBooks = _mockStorefrontService.GetAllBooks();
            var orders = _unitOfWork.OrderHeader.GetAll().OrderByDescending(order => order.OrderDate).ToList();
            var orderIds = orders.Select(order => order.Id).ToList();
            var details = _unitOfWork.OrderDetail.GetAll(detail => orderIds.Contains(detail.OrderHeaderId), includeProperties: "Product").ToList();

            var viewModel = new AdminDashboardViewModel
            {
                Metrics =
                [
                    new() { Label = "Books", Value = allBooks.Count.ToString(), AccentClass = "bg-primary-subtle text-primary" },
                    new() { Label = "Users", Value = _unitOfWork.ApplicationUser.GetAll().Count().ToString(), AccentClass = "bg-success-subtle text-success" },
                    new() { Label = "Orders", Value = orders.Count.ToString(), AccentClass = "bg-warning-subtle text-warning-emphasis" },
                    new() { Label = "Revenue", Value = orders.Sum(order => order.OrderTotal).ToString("C"), AccentClass = "bg-info-subtle text-info-emphasis" }
                ],
                SalesByMonth = orders
                    .GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month })
                    .OrderBy(group => group.Key.Year)
                    .ThenBy(group => group.Key.Month)
                    .TakeLast(6)
                    .Select(group => new SalesByMonthViewModel
                    {
                        Label = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMM yyyy"),
                        Revenue = group.Sum(order => order.OrderTotal),
                        Orders = group.Count()
                    })
                    .ToList(),
                Bestsellers = allBooks.OrderByDescending(book => book.UnitsSold).Take(5).ToList(),
                LowStockBooks = allBooks.OrderBy(book => book.StockCount).Take(5).ToList(),
                RecentOrders = orders.Take(5).Select(order => new OrderHistoryItemViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.OrderStatus ?? "Pending",
                    PaymentStatus = order.PaymentStatus ?? "Pending",
                    OrderTotal = order.OrderTotal,
                    TrackingNumber = order.TrackingNumber ?? "Pending assignment",
                    Lines = details.Where(detail => detail.OrderHeaderId == order.Id).Select(detail => new OrderHistoryLineViewModel
                    {
                        Title = detail.Product.Title,
                        ImageUrl = detail.Product.ImageUrl,
                        Quantity = detail.Count,
                        UnitPrice = detail.Price
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
