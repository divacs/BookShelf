using BookShelf.DataAccess.Repository.IRepository;
using BookShelfWeb.Services;
using BookShelfWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShelfWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMockStorefrontService _mockStorefrontService;

        public LibraryController(IUnitOfWork unitOfWork, IMockStorefrontService mockStorefrontService)
        {
            _unitOfWork = unitOfWork;
            _mockStorefrontService = mockStorefrontService;
        }

        public IActionResult Wishlist() =>
            View(new WishlistViewModel { Books = _mockStorefrontService.GetWishlistBooks() });

        public IActionResult Orders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var orders = _unitOfWork.OrderHeader.GetAll(order => order.ApplicationUserId == userId).OrderByDescending(order => order.OrderDate).ToList();
            var orderIds = orders.Select(order => order.Id).ToList();
            var details = _unitOfWork.OrderDetail.GetAll(detail => orderIds.Contains(detail.OrderHeaderId), includeProperties: "Product").ToList();

            return View(MapOrders(orders, details));
        }

        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var user = _unitOfWork.ApplicationUser.Get(applicationUser => applicationUser.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            var orders = _unitOfWork.OrderHeader.GetAll(order => order.ApplicationUserId == userId).OrderByDescending(order => order.OrderDate).ToList();
            var orderIds = orders.Select(order => order.Id).ToList();
            var details = _unitOfWork.OrderDetail.GetAll(detail => orderIds.Contains(detail.OrderHeaderId), includeProperties: "Product").ToList();

            return View(new UserProfileViewModel
            {
                User = user,
                WishlistCount = _mockStorefrontService.GetWishlistBooks().Count,
                CartItemsCount = _unitOfWork.ShoppingCart.GetAll(cart => cart.ApplicationUserId == userId).Sum(cart => cart.Count),
                TotalOrders = orders.Count,
                LifetimeSpend = orders.Sum(order => order.OrderTotal),
                RecentOrders = MapOrders(orders.Take(2).ToList(), details)
            });
        }

        private static List<OrderHistoryItemViewModel> MapOrders(IReadOnlyList<BookShelf.Models.Models.OrderHeader> orders, IReadOnlyList<BookShelf.Models.Models.OrderDetail> details) =>
            orders.Select(order => new OrderHistoryItemViewModel
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
            }).ToList();
    }
}
