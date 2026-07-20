using BookShelf.DataAccess.Repository.IRepository;
using BookShelf.Models.Models;
using BookShelfWeb.Configuration;
using BookShelfWeb.MockData;
using BookShelfWeb.ViewModels;
using Microsoft.Extensions.Options;

namespace BookShelfWeb.Services
{
    public class MockStorefrontService : IMockStorefrontService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReadOnlyDictionary<string, BookCatalogEntry> _catalogByIsbn;

        public MockStorefrontService(IUnitOfWork unitOfWork, IOptions<MockStorefrontOptions> options)
        {
            _unitOfWork = unitOfWork;
            IsEnabled = options.Value.Enabled;
            _catalogByIsbn = BookMockCatalog.Books.ToDictionary(book => book.ISBN, StringComparer.OrdinalIgnoreCase);
        }

        public bool IsEnabled { get; }

        public IReadOnlyList<string> GetCategories() =>
            GetAllBooks().Select(book => book.CategoryName).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(name => name).ToList();

        public IReadOnlyList<StoreBookViewModel> GetAllBooks()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return products.Where(product => _catalogByIsbn.ContainsKey(product.ISBN)).Select(MapBook).OrderBy(book => book.Product.Title).ToList();
        }

        public StoreBookViewModel? GetBookById(int productId)
        {
            var product = _unitOfWork.Product.Get(product => product.Id == productId, includeProperties: "Category");
            return product == null || !_catalogByIsbn.ContainsKey(product.ISBN) ? null : MapBook(product);
        }

        public StoreBookViewModel? GetBookByIsbn(string isbn)
        {
            var product = _unitOfWork.Product.Get(product => product.ISBN == isbn, includeProperties: "Category");
            return product == null || !_catalogByIsbn.ContainsKey(product.ISBN) ? null : MapBook(product);
        }

        public IReadOnlyList<StoreBookViewModel> GetFeaturedBooks(int take) => GetAllBooks().Where(book => book.IsFeatured).Take(take).ToList();

        public IReadOnlyList<StoreBookViewModel> GetBestsellers(int take) => GetAllBooks().Where(book => book.IsBestseller).OrderByDescending(book => book.UnitsSold).Take(take).ToList();

        public IReadOnlyList<StoreBookViewModel> GetNewArrivals(int take) => GetAllBooks().Where(book => book.IsNewRelease).OrderByDescending(book => _catalogByIsbn[book.Product.ISBN].AddedOnUtc).Take(take).ToList();

        public IReadOnlyList<StoreBookViewModel> GetRelatedBooks(StoreBookViewModel book, int take) =>
            GetAllBooks()
                .Where(candidate => candidate.Product.Id != book.Product.Id)
                .Where(candidate => candidate.CategoryName.Equals(book.CategoryName, StringComparison.OrdinalIgnoreCase) || candidate.Tags.Intersect(book.Tags, StringComparer.OrdinalIgnoreCase).Any())
                .Take(take)
                .ToList();

        public IReadOnlyList<StoreBookViewModel> SearchBooks(string? query, string? category)
        {
            var normalizedQuery = query?.Trim();
            var normalizedCategory = category?.Trim();

            return GetAllBooks()
                .Where(book => string.IsNullOrWhiteSpace(normalizedCategory) || book.CategoryName.Equals(normalizedCategory, StringComparison.OrdinalIgnoreCase))
                .Where(book => string.IsNullOrWhiteSpace(normalizedQuery) || book.Product.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) || book.Product.Author.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) || book.ShortDescription.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) || book.Tags.Any(tag => tag.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public IReadOnlyList<StoreBookViewModel> GetWishlistBooks() =>
            BookMockCatalog.WishlistIsbns.Select(GetBookByIsbn).Where(book => book != null).Cast<StoreBookViewModel>().ToList();

        public IReadOnlyList<(StoreBookViewModel Book, int Count)> GetPortfolioCartItems() =>
            BookMockCatalog.CartIsbnsWithCounts.Select(pair => (Book: GetBookByIsbn(pair.Key), Count: pair.Value)).Where(pair => pair.Book != null).Select(pair => (pair.Book!, pair.Count)).ToList();

        private StoreBookViewModel MapBook(Product product)
        {
            var metadata = _catalogByIsbn[product.ISBN];

            return new StoreBookViewModel
            {
                Product = product,
                CategoryName = product.Category?.Name ?? metadata.CategoryName,
                ShortDescription = metadata.ShortDescription,
                PublishedYear = metadata.PublishedYear,
                PageCount = metadata.PageCount,
                Publisher = metadata.Publisher,
                AverageRating = metadata.AverageRating,
                ReviewCount = metadata.ReviewCount,
                StockCount = metadata.StockCount,
                IsFeatured = metadata.IsFeatured,
                IsBestseller = metadata.IsBestseller,
                IsNewRelease = metadata.IsNewRelease,
                IsDiscounted = metadata.IsDiscounted,
                UnitsSold = metadata.UnitsSold,
                Tags = metadata.Tags
            };
        }
    }
}
