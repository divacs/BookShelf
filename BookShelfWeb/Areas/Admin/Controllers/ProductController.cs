
using BookShelf.DataAccess.Data;
using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelf.Models.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShelfWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                                    .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                return View(productVM);
            }

            if (productVM.Product.Id == 0)
            {
                // Create
                _unitOfWork.Product.Add(productVM.Product);
                TempData["success"] = "Product created successfully";
            }
            else
            {
                // Update
                _unitOfWork.Product.Update(productVM.Product);
                TempData["success"] = "Product updated successfully";
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
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

        //    // Ponovo popuni CategoryList ako validacija ne uspe
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

    }
}

