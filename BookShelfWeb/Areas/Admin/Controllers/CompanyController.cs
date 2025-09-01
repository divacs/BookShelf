
using BookShelf.DataAccess.Data;
using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelf.Models.Models.ViewModels;
using BookShelf.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookShelfWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // we are returning company index page
        // we are returning company index page
        public IActionResult Index()
        {
            var companies = _unitOfWork.Company.GetAll().ToList();
            var categories = _unitOfWork.Category.GetAll()
                .ToDictionary(c => c.Id, c => c.Name);

            //foreach (var company in companies)
            //{
            //    if (categories.ContainsKey(company.CategoryId))
            //    {
            //        company.Category = new Category
            //        {
            //            Id = company.CategoryId,
            //            Name = categories[company.CategoryId]
            //        };
            //    }
            //}

            return View(companies);
        }

        public IActionResult Upsert(int? id) // create + update
        {
            
            if (id == null || id == 0)
            {
                // create
                return View(new Company());
            }
            else
            {
                // update
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }


            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company companyObj)
        {
            // Proveri validnost forme
            if (ModelState.IsValid)
            {

                // Dodaj ili update proizvoda
                if (companyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);
                    TempData["success"] = "Company updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Ako forma nije validna, vrati ponovo view sa kategorijama   
                return View(companyObj);
            }
        }

        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    Company company = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (company == null) return NotFound();

        //    CompanyVM vm = new CompanyVM
        //    {
        //        Company = company,
        //        CategoryList = _unitOfWork.Category.GetAll()
        //                            .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
        //                            .ToList()
        //    };
        //    return View(vm);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(CompanyVM vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Company.Update(vm.Company);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company updated successfully";
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
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyFromDb == null)
            {
                return NotFound();
            }
            return View(companyFromDb);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Company company = _unitOfWork.Company.Get(u => u.Id == id);
            if (company == null) return NotFound();

            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companyList });
        }

        #endregion
    }
}

