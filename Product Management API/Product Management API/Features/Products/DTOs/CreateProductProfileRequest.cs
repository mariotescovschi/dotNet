using Product_Management_API.Enums;

namespace Product_Management_API.DTOs;

public class CreateProductProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; } = 1;
}