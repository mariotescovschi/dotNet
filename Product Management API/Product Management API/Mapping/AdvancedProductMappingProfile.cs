using AutoMapper;
using Product_Management_API.Constants;
using Product_Management_API.DTOs;
using Product_Management_API.Entities;
using Product_Management_API.Enums;

namespace Product_Management_API.Mapping;

public class AdvancedProductMappingProfile : Profile
{
    public AdvancedProductMappingProfile()
    {
        CreateMap<CreateProductProfileRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.StockQuantity > 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Product, ProductProfileDto>()
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom<PriceFormatterResolver>())
            .ForMember(dest => dest.ProductAge, opt => opt.MapFrom<ProductAgeResolver>())
            .ForMember(dest => dest.BrandInitials, opt => opt.MapFrom<BrandInitialsResolver>())
            .ForMember(dest => dest.AvailabilityStatus, opt => opt.MapFrom<AvailabilityStatusResolver>())
            .ForMember(dest => dest.Price, opt => opt.MapFrom<ConditionalPriceResolver>())
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<ConditionalImageUrlResolver>());
    }
}

/// <summary>
/// Maps ProductCategory to display names: Electronics → "Electronics & Technology", etc.
/// </summary>
public class CategoryDisplayResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        return source.Category switch
        {
            ProductCategory.Electronics => ProductConstants.CategoryDisplayElectronics,
            ProductCategory.Clothing => ProductConstants.CategoryDisplayClothing,
            ProductCategory.Books => ProductConstants.CategoryDisplayBooks,
            ProductCategory.Home => ProductConstants.CategoryDisplayHome,
            _ => ProductConstants.CategoryDisplayDefault
        };
    }
}

/// <summary>
/// Formats price as currency (C2 format).
/// </summary>
public class PriceFormatterResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        return source.Price.ToString(ProductConstants.CurrencyFormat);
    }
}

/// <summary>
/// Calculates product age: New Release (&lt;30 days), X months old, X years old, or Classic (≥5 years).
/// </summary>
public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        var daysDifference = (DateTime.UtcNow - source.ReleaseDate).TotalDays;

        if (daysDifference < ProductConstants.NewReleaseThresholdDays)
            return ProductConstants.ProductAgeNewRelease;

        if (daysDifference < ProductConstants.MonthsOldThresholdDays)
        {
            var months = (int)(daysDifference / ProductConstants.AverageDaysPerMonth);
            return $"{months} month{(months != 1 ? "s" : "")} old";
        }

        if (daysDifference < ProductConstants.YearsOldThresholdDays)
        {
            var years = (int)(daysDifference / ProductConstants.AverageDaysPerYear);
            return $"{years} year{(years != 1 ? "s" : "")} old";
        }

        return ProductConstants.ProductAgeClassic;
    }
}

/// <summary>
/// Extracts brand initials: "Apple Inc" → "AI", "Nike" → "N", empty → "?".
/// </summary>
public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.Brand))
            return ProductConstants.BrandInitialsPlaceholder;

        var words = source.Brand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length < 2
            ? words[0][0].ToString().ToUpper()
            : $"{words[0][0]}{words[^1][0]}".ToUpper();
    }
}

/// <summary>
/// Maps availability status: "Out of Stock", "Unavailable", "Last Item", "Limited Stock", or "In Stock".
/// </summary>
public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        if (!source.IsAvailable)
            return ProductConstants.AvailabilityStatusOutOfStock;

        return source.StockQuantity switch
        {
            ProductConstants.ZeroStock => ProductConstants.AvailabilityStatusUnavailable,
            ProductConstants.LastItemStock => ProductConstants.AvailabilityStatusLastItem,
            <= ProductConstants.LimitedStockThreshold => ProductConstants.AvailabilityStatusLimitedStock,
            _ => ProductConstants.AvailabilityStatusInStock
        };
    }
}

/// <summary>
/// Applies 10% discount for Home category, returns actual price otherwise.
/// </summary>
public class ConditionalPriceResolver : IValueResolver<Product, ProductProfileDto, decimal>
{
    public decimal Resolve(Product source, ProductProfileDto destination, decimal destMember, ResolutionContext context)
    {
        return source.Category == ProductCategory.Home
            ? source.Price * ProductConstants.HomeDiscountMultiplier
            : source.Price;
    }
}

/// <summary>
/// Returns ImageUrl for Electronics/Clothing/Books, null for Home category (content filtering).
/// </summary>
public class ConditionalImageUrlResolver : IValueResolver<Product, ProductProfileDto, string?>
{
    public string? Resolve(Product source, ProductProfileDto destination, string? destMember, ResolutionContext context)
    {
        return source.Category == ProductCategory.Home ? null : source.ImageUrl;
    }
}