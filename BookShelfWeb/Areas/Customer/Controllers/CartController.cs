using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelf.Models.Models.ViewModels;
using BookShelf.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

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
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var shoppingCartVM = new ShoppingCartVM 
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shoppingCartVM);
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

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
