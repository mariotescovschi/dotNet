using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Product_Management_API.Constants;
using Product_Management_API.Data;
using Product_Management_API.DTOs;
using Product_Management_API.Enums;

namespace Product_Management_API.Validators;

public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    private readonly ApplicationContext _context;
    private readonly ILogger<CreateProductProfileValidator> _logger;

    public CreateProductProfileValidator(ApplicationContext context, ILogger<CreateProductProfileValidator> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeRules();
    }

    private void InitializeRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ProductConstants.NameRequiredMessage)
            .Length(ProductConstants.NameMinLength, ProductConstants.NameMaxLength)
            .WithMessage($"Product name must be between {ProductConstants.NameMinLength} and {ProductConstants.NameMaxLength} characters")
            .Must(BeValidName).WithMessage(ProductConstants.NameInappropriateContentMessage);

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage(ProductConstants.BrandRequiredMessage)
            .Length(ProductConstants.BrandMinLength, ProductConstants.BrandMaxLength)
            .WithMessage($"Brand name must be between {ProductConstants.BrandMinLength} and {ProductConstants.BrandMaxLength} characters")
            .Must(BeValidBrandName)
            .WithMessage(ProductConstants.BrandInvalidCharactersMessage)
            .MinimumLength(3).WithMessage(ProductConstants.BrandMinLengthClothingMessage)
            .When(r => r.Category == ProductCategory.Clothing);

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage(ProductConstants.SkuRequiredMessage)
            .Must(BeValidSku)
            .WithMessage($"SKU must be alphanumeric with hyphens, between {ProductConstants.SkuMinLength} and {ProductConstants.SkuMaxLength} characters")
            .MustAsync(BeUniqueSku).WithMessage(ProductConstants.SkuUniqueMessage);

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage(ProductConstants.CategoryValidMessage);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(ProductConstants.PriceGreaterThanZeroMessage)
            .LessThan(ProductConstants.MaxPrice).WithMessage($"Price must be less than {ProductConstants.MaxPrice:C2}")
            .GreaterThanOrEqualTo(ProductConstants.ElectronicsMinPrice)
            .WithMessage($"Electronics products must have a minimum price of {ProductConstants.ElectronicsMinPrice:C2}")
            .When(r => r.Category == ProductCategory.Electronics)
            .LessThanOrEqualTo(ProductConstants.HomeMaxPrice).WithMessage(ProductConstants.HomeMaxPriceMessage)
            .When(r => r.Category == ProductCategory.Home);

        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(ProductConstants.ReleaseDateFutureMessage)
            .Must(BeValidReleaseDate).WithMessage($"Release date cannot be before year {ProductConstants.MinYear}")
            .Must((_, releaseDate) => BeWithinLastFiveYears(releaseDate)).WithMessage($"Electronics products must be released within the last {ProductConstants.ElectronicsLastYears} years")
            .When(r => r.Category == ProductCategory.Electronics);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage(ProductConstants.StockNegativeMessage)
            .LessThanOrEqualTo(ProductConstants.MaxStockQuantity).WithMessage($"Stock quantity cannot exceed {ProductConstants.MaxStockQuantity}")
            .LessThanOrEqualTo(ProductConstants.ExpensiveProductMaxStock).WithMessage($"Expensive products (>{ProductConstants.ExpensiveProductThreshold}) must have limited stock (≤{ProductConstants.ExpensiveProductMaxStock} units)")
            .When(r => r.Price > ProductConstants.ExpensiveProductThreshold);

        RuleFor(x => x.ImageUrl)
            .Must(BeValidImageUrl)
            .WithMessage(ProductConstants.ImageUrlValidMessage)
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        RuleFor(x => x)
            .MustAsync(BeUniqueName).WithMessage(ProductConstants.NameUniquePerBrandMessage);

        RuleFor(x => x)
            .MustAsync(PassBusinessRules).WithMessage(ProductConstants.BusinessRulesFailedMessage);

        RuleFor(x => x.Name)
            .Must(ContainTechnologyKeywords)
            .WithMessage(ProductConstants.ElectronicsTechnologyKeywordsMessage)
            .When(r => r.Category == ProductCategory.Electronics);

        RuleFor(x => x.Name)
            .Must(BeAppropriateForHome)
            .WithMessage(ProductConstants.HomeInappropriateContentMessage)
            .When(r => r.Category == ProductCategory.Home);
    }

    private bool BeValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return ProductConstants.InappropriateWords.All(word => !name.ToLower().Contains(word.ToLower()));
    }

    private bool BeValidBrandName(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            return false;

        return Regex.IsMatch(brand, ProductConstants.BrandNamePattern);
    }

    private bool BeValidSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;

        return Regex.IsMatch(sku, ProductConstants.SkuPattern);
    }

    private async Task<bool> BeUniqueSku(string sku, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return true;

        try
        {
            bool exists = await _context.Products
                .AnyAsync(p => p.Sku == sku, cancellationToken);

            if (exists)
            {
                _logger.LogWarning(ProductLogEvents.SKUValidationPerformed,
                    "SKU '{SKU}' already exists in the system", sku);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ProductLogEvents.DatabaseOperationCompleted,
                ex, "Error checking SKU uniqueness for '{SKU}'", sku);
            throw;
        }
    }

    private async Task<bool> BeUniqueName(CreateProductProfileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Brand))
            return true;

        try
        {
            bool exists = await _context.Products
                .AnyAsync(p => p.Name == request.Name && p.Brand == request.Brand, cancellationToken);

            if (exists)
            {
                _logger.LogWarning(ProductLogEvents.ProductValidationFailed,
                    "Product with name '{Name}' and brand '{Brand}' already exists", request.Name, request.Brand);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ProductLogEvents.DatabaseOperationCompleted,
                ex, "Error checking product name uniqueness for '{Name}' in brand '{Brand}'", request.Name,
                request.Brand);
            throw;
        }
    }

    private bool BeValidImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return true;

        try
        {
            var uri = new Uri(imageUrl);

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return false;

            return Regex.IsMatch(imageUrl.ToLower(), ProductConstants.ImageExtensionsPattern);
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> PassBusinessRules(CreateProductProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await CheckDailyProductLimit(cancellationToken))
                return false;

            if (!CheckElectronicsMinPrice(request))
                return false;

            if (!CheckHomeProductRestrictions(request))
                return false;

            if (!CheckHighValueProductStock(request))
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ProductLogEvents.ProductValidationFailed,
                ex, "Error during business rules validation");
            throw;
        }
    }

    private async Task<bool> CheckDailyProductLimit(CancellationToken cancellationToken)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            int productsAddedToday = await _context.Products
                .CountAsync(p => p.CreatedAt.Date == today, cancellationToken);

            if (productsAddedToday >= ProductConstants.MaxDailyProducts)
            {
                _logger.LogWarning(ProductLogEvents.ProductCreationStarted,
                    "Daily product addition limit reached. Products added today: {Count}", productsAddedToday);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ProductLogEvents.DatabaseOperationCompleted,
                ex, "Error checking daily product limit");
            throw;
        }
    }

    private bool CheckElectronicsMinPrice(CreateProductProfileRequest request)
    {
        if (request.Category == ProductCategory.Electronics && request.Price < ProductConstants.ElectronicsMinPrice)
        {
            _logger.LogWarning(ProductLogEvents.ProductValidationFailed,
                "Electronics product '{Name}' has price {Price} below minimum {MinPrice}",
                request.Name, request.Price.ToString("C2"), ProductConstants.ElectronicsMinPrice.ToString("C2"));
            return false;
        }

        return true;
    }

    private bool CheckHomeProductRestrictions(CreateProductProfileRequest request)
    {
        if (request.Category != ProductCategory.Home)
            return true;

        bool hasRestrictedWords = ProductConstants.RestrictedHomeWords
            .Any(word => request.Name.ToLower().Contains(word.ToLower()));

        if (hasRestrictedWords)
        {
            _logger.LogWarning(ProductLogEvents.ProductValidationFailed,
                "Home product '{Name}' contains restricted words", request.Name);
            return false;
        }

        return true;
    }

    private bool CheckHighValueProductStock(CreateProductProfileRequest request)
    {
        if (request.Price > ProductConstants.HighValueProductThreshold && request.StockQuantity > ProductConstants.HighValueProductMaxStock)
        {
            _logger.LogWarning(ProductLogEvents.StockValidationPerformed,
                "High-value product '{Name}' with price {Price} exceeds maximum stock limit of {MaxStock}",
                request.Name, request.Price.ToString("C2"), ProductConstants.HighValueProductMaxStock);
            return false;
        }

        return true;
    }

    private static bool BeValidReleaseDate(DateTime releaseDate)
    {
        return releaseDate.Year >= ProductConstants.MinYear;
    }

    private static bool BeWithinLastFiveYears(DateTime releaseDate)
    {
        var fiveYearsAgo = DateTime.UtcNow.AddYears(-5);
        return releaseDate >= fiveYearsAgo;
    }

    private bool ContainTechnologyKeywords(string name)
    {
        return ProductConstants.TechnologyKeywords.Any(keyword => name.ToLower().Contains(keyword));
    }

    private bool BeAppropriateForHome(string name)
    {
        return BeValidName(name) && !ProductConstants.RestrictedHomeWords.Any(word => name.ToLower().Contains(word));
    }
}