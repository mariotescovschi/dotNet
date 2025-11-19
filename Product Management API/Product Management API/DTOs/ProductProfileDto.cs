using Product_Management_API.Enums;

namespace Product_Management_API.DTOs;

public class ProductProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }

    // Derived properties (populated by AutoMapper resolvers)
    public string CategoryDisplayName { get; set; }
    public string FormattedPrice { get; set; }
    public string ProductAge { get; set; }
    public string BrandInitials { get; set; }
    public string AvailabilityStatus { get; set; }
}