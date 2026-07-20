using BookShelfWeb.ViewModels;

namespace BookShelfWeb.Services
{
    public interface IMockStorefrontService
    {
        bool IsEnabled { get; }
        IReadOnlyList<string> GetCategories();
        IReadOnlyList<StoreBookViewModel> GetAllBooks();
        StoreBookViewModel? GetBookById(int productId);
        StoreBookViewModel? GetBookByIsbn(string isbn);
        IReadOnlyList<StoreBookViewModel> GetFeaturedBooks(int take);
        IReadOnlyList<StoreBookViewModel> GetBestsellers(int take);
        IReadOnlyList<StoreBookViewModel> GetNewArrivals(int take);
        IReadOnlyList<StoreBookViewModel> GetRelatedBooks(StoreBookViewModel book, int take);
        IReadOnlyList<StoreBookViewModel> SearchBooks(string? query, string? category);
        IReadOnlyList<StoreBookViewModel> GetWishlistBooks();
        IReadOnlyList<(StoreBookViewModel Book, int Count)> GetPortfolioCartItems();
    }
}
