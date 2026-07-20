using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelfWeb.Services;
using BookShelfWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMockStorefrontService _mockStorefrontService;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMockStorefrontService mockStorefrontService)
    {
        _logger = logger;
        _mockStorefrontService = mockStorefrontService;
    }

    public IActionResult Index()
    {
        return View(new StorefrontHomeViewModel
        {
            Categories = _mockStorefrontService.GetCategories(),
            FeaturedBooks = _mockStorefrontService.GetFeaturedBooks(4),
            Bestsellers = _mockStorefrontService.GetBestsellers(4),
            NewArrivals = _mockStorefrontService.GetNewArrivals(4),
            AllBooks = _mockStorefrontService.GetAllBooks()
        });
    }

    public IActionResult Details(int productId)
    {
        var book = _mockStorefrontService.GetBookById(productId);
        if (book == null)
        {
            return NotFound();
        }

        return View(new BookDetailsViewModel
        {
            Book = book,
            ShoppingCart = new ShoppingCart
            {
                Product = book.Product,
                ProductId = productId,
                Count = 1
            },
            RelatedBooks = _mockStorefrontService.GetRelatedBooks(book, 4)
        });
    }

    public IActionResult Search(string? query, string? category)
    {
        return View(new SearchResultsViewModel
        {
            Query = query,
            Category = category,
            Categories = _mockStorefrontService.GetCategories(),
            Results = _mockStorefrontService.SearchBooks(query, category)
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
