using MediatR;
using Product_Management_API.DTOs;

namespace Product_Management_API.Commands;

public class CreateProductCommand : IRequest<ProductProfileDto>
{
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public int Category { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }

    public CreateProductCommand()
    {
    }

    public CreateProductCommand(CreateProductProfileRequest request)
    {
        Name = request.Name;
        Brand = request.Brand;
        SKU = request.SKU;
        Category = (int)request.Category;
        Price = request.Price;
        ReleaseDate = request.ReleaseDate;
        ImageUrl = request.ImageUrl;
        StockQuantity = request.StockQuantity;
    }
}