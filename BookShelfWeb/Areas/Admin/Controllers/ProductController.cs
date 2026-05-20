
using BookShelf.DataAccess.Data;
using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelf.Models.Models.ViewModels;
using BookShelf.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookShelfWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private const long MaxImageSize = 2 * 1024 * 1024;
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png"];
        private static readonly string[] AllowedImageContentTypes = ["image/jpeg", "image/png"];

        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(ApplicationDbContext db, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        // we are returning product index page
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll().ToList();
            var categories = _unitOfWork.Category.GetAll().ToDictionary(c => c.Id, c => c.Name);

            foreach (var product in products)
            {
                if (categories.ContainsKey(product.CategoryId))
                {
                    product.Category = new Category { Id = product.CategoryId, Name = categories[product.CategoryId] };
                }
            }

            return View(products);
        }


        public IActionResult Upsert(int? id) // create + update
        {
  
            ProductVM productVM = new()
            {

                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Product = new Product()  // to show category name instead of id
            };
            if (id == null || id == 0)
            {
                // create
                return View(productVM);
            }
            else
            {
                // update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }


            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            string? existingImageUrl = null;
            if (productVM.Product.Id != 0)
            {
                existingImageUrl = _db.Products
                    .AsNoTracking()
                    .Where(u => u.Id == productVM.Product.Id)
                    .Select(u => u.ImageUrl)
                    .FirstOrDefault();
            }

            if (file != null)
            {
                ValidateImageFile(file);
            }

            // Proveri validnost forme
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                // if new file exists save new img and then delete old img
                if (file != null)
                {
                    // save new file
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!Directory.Exists(productPath))
                    {
                        Directory.CreateDirectory(productPath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"/images/product/" + fileName;

                    // delete old img after the new img is saved successfully
                    if (!string.IsNullOrEmpty(existingImageUrl))
                    {
                        string oldFilePath = Path.Combine(wwwRootPath, existingImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                else if (productVM.Product.Id != 0)
                {
                    productVM.Product.ImageUrl = existingImageUrl ?? string.Empty;
                }

                // add or update product
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            // if form is not valid, repopulate CategoryList and return the view
            if (productVM.Product.Id != 0)
            {
                productVM.Product.ImageUrl = existingImageUrl ?? string.Empty;
            }
            productVM.CategoryList = _unitOfWork.Category.GetAll()
                                        .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            return View(productVM);
        }

        private void ValidateImageFile(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError("Product.ImageUrl", "Only JPG, JPEG, and PNG images are allowed.");
            }

            if (!AllowedImageContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Product.ImageUrl", "Only JPEG and PNG image content types are allowed.");
            }

            if (file.Length > MaxImageSize)
            {
                ModelState.AddModelError("Product.ImageUrl", "Image size must be 2 MB or smaller.");
            }
        }

        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    Product product = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (product == null) return NotFound();

        //    ProductVM vm = new ProductVM
        //    {
        //        Product = product,
        //        CategoryList = _unitOfWork.Category.GetAll()
        //                            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
        //                            .ToList()
        //    };
        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(ProductVM vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(vm.Product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // 
        //    vm.CategoryList = _unitOfWork.Category.GetAll()
        //                            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
        //                            .ToList();
        //    return View(vm);
        //}

        // Delete GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null) return NotFound();

            ProductVM vm = new ProductVM
            {
                Product = product
            };
            return View(vm);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null) return NotFound();

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll().ToList();
            return Json(new { data = productList });
        }

        #endregion
    }
}

