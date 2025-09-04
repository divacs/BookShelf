using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using Microsoft.AspNetCore.Mvc;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var productList = _unitOfWork.Product.GetAll();
        return View(productList);
    }

    // GET: Details
    public IActionResult Details(int productId)
    {
        var product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category");

        if (product == null)
            return NotFound();

        var cart = new ShoppingCart
        {
            Product = product,
            ProductId = productId,
            Count = 1
        };

        return View(cart);
    }
}
