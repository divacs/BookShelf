using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelf.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookShelfWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Cart/Index
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product"
            ).ToList();

            return View(cartItems);
        }

        // POST: Cart/Plus/5
        public IActionResult Plus(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartItem != null)
            {
                cartItem.Count += 1;
                _unitOfWork.Save();
            }

            UpdateSessionCart(cartItem?.ApplicationUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Minus/5
        public IActionResult Minus(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartItem != null)
            {
                if (cartItem.Count <= 1)
                {
                    _unitOfWork.ShoppingCart.Remove(cartItem);
                }
                else
                {
                    cartItem.Count -= 1;
                }
                _unitOfWork.Save();
            }

            UpdateSessionCart(cartItem?.ApplicationUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/Remove/5
        public IActionResult Remove(int cartId)
        {
            var cartItem = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartItem != null)
            {
                _unitOfWork.ShoppingCart.Remove(cartItem);
                _unitOfWork.Save();
            }

            UpdateSessionCart(cartItem?.ApplicationUserId);
            return RedirectToAction(nameof(Index));
        }

        private void UpdateSessionCart(string? userId)
        {
            if (userId != null)
            {
                int cartCount = _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == userId)
                    .Sum(u => u.Count);

                HttpContext.Session.SetInt32(SD.SessionCart, cartCount);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int count = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Nađi item u korpi ako već postoji
            var cartItem = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == productId
            );

            if (cartItem == null)
            {
                // Ako ne postoji → napravi novi
                cartItem = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    Count = count
                };
                _unitOfWork.ShoppingCart.Add(cartItem);
            }
            else
            {
                // Ako već postoji → samo povećaj Count
                cartItem.Count += count;
            }

            _unitOfWork.Save();

            // Ažuriraj Session broj artikala
            int cartCount = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == userId)
                .Sum(u => u.Count);

            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

            return RedirectToAction("Index", "Cart");
        }


    }
}
