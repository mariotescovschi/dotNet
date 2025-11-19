using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Product_Management_API.Commands;
using Product_Management_API.Constants;
using Product_Management_API.Data;
using Product_Management_API.DTOs;
using Product_Management_API.Entities;
using Product_Management_API.Enums;
using Product_Management_API.Exceptions;

namespace Product_Management_API.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductProfileDto>
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public CreateProductHandler(ApplicationContext context, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductProfileDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var skuExists = await _context.Products.AnyAsync(p => p.SKU == request.SKU, cancellationToken);
        if (skuExists)
        {
            throw new ValidationException($"A product with SKU '{request.SKU}' already exists. SKU must be unique.");
        }

        var createRequest = new CreateProductProfileRequest
        {
            Name = request.Name,
            Brand = request.Brand,
            SKU = request.SKU,
            Category = (ProductCategory)request.Category,
            Price = request.Price,
            ReleaseDate = request.ReleaseDate,
            ImageUrl = request.ImageUrl,
            StockQuantity = request.StockQuantity
        };

        var product = _mapper.Map<Product>(createRequest);
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"{ProductConstants.LoggerPrefix} {ProductConstants.LoggerSuccessMessage}");
        Console.WriteLine($"  - {ProductConstants.LoggerIdLabel}: {product.Id}");
        Console.WriteLine($"  - {ProductConstants.LoggerNameLabel}: {product.Name}");
        Console.WriteLine($"  - {ProductConstants.LoggerBrandLabel}: {product.Brand}");
        Console.WriteLine($"  - {ProductConstants.LoggerCategoryLabel}: {product.Category}");
        Console.WriteLine($"  - {ProductConstants.LoggerSkuLabel}: {product.SKU}");
        Console.WriteLine($"  - {ProductConstants.LoggerPriceLabel}: {product.Price:C2}");
        Console.WriteLine($"  - {ProductConstants.LoggerStockQuantityLabel}: {product.StockQuantity}");
        Console.WriteLine($"  - {ProductConstants.LoggerCreatedAtLabel}: {product.CreatedAt:O}");

        _cache.Remove(ProductConstants.AllProductsCacheKey);

        return _mapper.Map<ProductProfileDto>(product);
    }
}