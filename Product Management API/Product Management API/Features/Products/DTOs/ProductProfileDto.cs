namespace Product_Management_API.DTOs;

public class ProductProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }

    // Derived properties (populated by AutoMapper resolvers)
    public string CategoryDisplayName { get; set; } = string.Empty;
    public string FormattedPrice { get; set; } = string.Empty;
    public string ProductAge { get; set; } = string.Empty;
    public string BrandInitials { get; set; } = string.Empty;
    public string AvailabilityStatus { get; set; } = string.Empty;
}