using System.ComponentModel.DataAnnotations;
using Product_Management_API.Constants;
using Product_Management_API.Enums;

namespace Product_Management_API.Entities;

public class Product
{
    public Guid Id { get; set; }

    [MaxLength(ProductConstants.NameMaxLengthDb)] public string Name { get; set; } = string.Empty;

    [MaxLength(ProductConstants.BrandMaxLengthDb)] public string Brand { get; set; } = string.Empty;

    [MaxLength(ProductConstants.SkuMaxLengthDb)] public string Sku { get; set; } = string.Empty;

    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }

    [MaxLength(ProductConstants.ImageUrlMaxLengthDb)] public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}