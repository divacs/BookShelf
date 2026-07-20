namespace BookShelfWeb.MockData
{
    public class BookCatalogEntry
    {
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string ShortDescription { get; init; } = string.Empty;
        public string ISBN { get; init; } = string.Empty;
        public double ListPrice { get; init; }
        public double Price { get; init; }
        public double Price50 { get; init; }
        public double Price100 { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
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
        public DateTime AddedOnUtc { get; init; }
        public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    }
}
