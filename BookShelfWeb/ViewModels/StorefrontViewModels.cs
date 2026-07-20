using BookShelf.Models.Models;

namespace BookShelfWeb.ViewModels
{
    public class StoreBookViewModel
    {
        public Product Product { get; init; } = new();
        public string CategoryName { get; init; } = string.Empty;
        public string ShortDescription { get; init; } = string.Empty;
        public int PublishedYear { get; init; }
        public int PageCount { get; init; }
        public string Publisher { get; init; } = string.Empty;
        public double AverageRating { get; init; }
        public int ReviewCount { get; init; }
        public int StockCount { get; init; }
        public bool IsFeatured { get; init; }
        public bool IsBestseller { get; init; }
        public bool IsNewRelease { get; init; }
        public bool IsDiscounted { get; init; }
        public int UnitsSold { get; init; }
        public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    }

    public class StorefrontHomeViewModel
    {
        public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
        public IReadOnlyList<StoreBookViewModel> FeaturedBooks { get; init; } = Array.Empty<StoreBookViewModel>();
        public IReadOnlyList<StoreBookViewModel> Bestsellers { get; init; } = Array.Empty<StoreBookViewModel>();
        public IReadOnlyList<StoreBookViewModel> NewArrivals { get; init; } = Array.Empty<StoreBookViewModel>();
        public IReadOnlyList<StoreBookViewModel> AllBooks { get; init; } = Array.Empty<StoreBookViewModel>();
    }

    public class BookDetailsViewModel
    {
        public ShoppingCart ShoppingCart { get; init; } = new();
        public StoreBookViewModel Book { get; init; } = new();
        public IReadOnlyList<StoreBookViewModel> RelatedBooks { get; init; } = Array.Empty<StoreBookViewModel>();
    }

    public class SearchResultsViewModel
    {
        public string? Query { get; init; }
        public string? Category { get; init; }
        public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
        public IReadOnlyList<StoreBookViewModel> Results { get; init; } = Array.Empty<StoreBookViewModel>();
    }

    public class WishlistViewModel
    {
        public IReadOnlyList<StoreBookViewModel> Books { get; init; } = Array.Empty<StoreBookViewModel>();
    }

    public class OrderHistoryLineViewModel
    {
        public string Title { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public double UnitPrice { get; init; }
    }

    public class OrderHistoryItemViewModel
    {
        public int OrderId { get; init; }
        public DateTime OrderDate { get; init; }
        public string Status { get; init; } = string.Empty;
        public string PaymentStatus { get; init; } = string.Empty;
        public double OrderTotal { get; init; }
        public string TrackingNumber { get; init; } = string.Empty;
        public IReadOnlyList<OrderHistoryLineViewModel> Lines { get; init; } = Array.Empty<OrderHistoryLineViewModel>();
    }

    public class UserProfileViewModel
    {
        public ApplicationUser User { get; init; } = new();
        public int WishlistCount { get; init; }
        public int CartItemsCount { get; init; }
        public int TotalOrders { get; init; }
        public double LifetimeSpend { get; init; }
        public IReadOnlyList<OrderHistoryItemViewModel> RecentOrders { get; init; } = Array.Empty<OrderHistoryItemViewModel>();
    }

    public class AdminMetricViewModel
    {
        public string Label { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
        public string AccentClass { get; init; } = string.Empty;
    }

    public class SalesByMonthViewModel
    {
        public string Label { get; init; } = string.Empty;
        public double Revenue { get; init; }
        public int Orders { get; init; }
    }

    public class AdminDashboardViewModel
    {
        public IReadOnlyList<AdminMetricViewModel> Metrics { get; init; } = Array.Empty<AdminMetricViewModel>();
        public IReadOnlyList<SalesByMonthViewModel> SalesByMonth { get; init; } = Array.Empty<SalesByMonthViewModel>();
        public IReadOnlyList<StoreBookViewModel> Bestsellers { get; init; } = Array.Empty<StoreBookViewModel>();
        public IReadOnlyList<StoreBookViewModel> LowStockBooks { get; init; } = Array.Empty<StoreBookViewModel>();
        public IReadOnlyList<OrderHistoryItemViewModel> RecentOrders { get; init; } = Array.Empty<OrderHistoryItemViewModel>();
    }
}
