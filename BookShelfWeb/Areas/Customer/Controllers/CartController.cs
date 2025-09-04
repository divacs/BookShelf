using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
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

        // GET: prikaz svih stavki u korpi
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product"
            );

            return View(cartItems);
        }

        // POST: dodavanje u korpu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int count)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == productId
            );

            if (cart != null)
            {
                cart.Count += count;
                _unitOfWork.ShoppingCart.Update(cart);
            }
            else
            {
                var newCart = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    Count = count
                };
                _unitOfWork.ShoppingCart.Add(newCart);
            }

            _unitOfWork.Save();
            TempData["success"] = "Cart updated successfully";

            // uvek se vraća na Index koji koristi IEnumerable<ShoppingCart>
            return RedirectToAction("Index");
        }
    }
}
