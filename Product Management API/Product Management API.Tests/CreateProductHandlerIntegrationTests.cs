using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Product_Management_API.Commands;
using Product_Management_API.Constants;
using Product_Management_API.Data;
using Product_Management_API.DTOs;
using Product_Management_API.Entities;
using Product_Management_API.Enums;
using Product_Management_API.Exceptions;
using Product_Management_API.Handlers;
using Product_Management_API.Mapping;
using Xunit;

namespace Product_Management_API.Tests;

/// <summary>
/// Integration tests for CreateProductHandler covering mapping, logging, validation, and conditional logic.
/// </summary>
public class CreateProductHandlerIntegrationTests : IDisposable
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<CreateProductHandler>> _mockLogger;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerIntegrationTests()
    {
        // Setup: in-memory database with a unique name
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: $"ProductDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationContext(options);

        // Setup: AutoMapper with both profiles
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<AdvancedProductMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        // Setup: memory cache
        _cache = new MemoryCache(new MemoryCacheOptions());

        // Setup: mocked logger
        _mockLogger = new Mock<ILogger<CreateProductHandler>>();

        // Instantiate the handler with all dependencies
        _handler = new CreateProductHandler(_context, _mapper, _cache, _mockLogger.Object);
    }

    /// <summary>
    /// Test 1: Verify valid Electronics product request creates product with correct mappings.
    /// Asserts CategoryDisplayName, result type, BrandInitials, ProductAge, FormattedPrice, and AvailabilityStatus.
    /// Verifies ProductCreationStarted log called once.
    /// </summary>
    [Fact]
    public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
    {
        // Arrange: Create a valid Electronics request
        var request = new CreateProductCommand
        {
            Name = "Wireless Headphones",
            Brand = "Tech Innovations",
            Sku = "ELEC-WH-001",
            Category = (int)ProductCategory.Electronics,
            Price = 199.99m,
            ReleaseDate = DateTime.UtcNow.AddDays(-15), // 15 days old (New Release)
            ImageUrl = "https://example.com/headphones.jpg",
            StockQuantity = 50
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert: Check CategoryDisplayName ("Electronics & Technology")
        Assert.Equal(ProductConstants.CategoryDisplayElectronics, result.CategoryDisplayName);

        // Assert: result type should be ProductProfileDto (which is implicitly the type of result)
        Assert.NotNull(result);
        Assert.IsType<ProductProfileDto>(result);

        // Assert: BrandInitials (for two-word brand "Tech Innovations" should be "TI")
        Assert.Equal("TI", result.BrandInitials);

        // Assert: ProductAge calculation (should be "New Release" for 15 days old)
        Assert.Equal(ProductConstants.ProductAgeNewRelease, result.ProductAge);

        // Assert: FormattedPrice contains the price value (locale-independent check)
        Assert.Contains("199", result.FormattedPrice);
        Assert.NotEmpty(result.FormattedPrice);

        // Assert: AvailabilityStatus based on stock (50 items = "In Stock")
        Assert.Equal(ProductConstants.AvailabilityStatusInStock, result.AvailabilityStatus);

        // Assert: Product is available
        Assert.True(result.IsAvailable);

        // Verify Logging: ProductCreationStarted log called once
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                ProductLogEvents.ProductCreationStarted,
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 2: Verify duplicate SKU throws ValidationException with appropriate logging.
    /// Checks that exception message contains "already exists".
    /// Verifies ProductValidationFailed log called once.
    /// </summary>
    [Fact]
    public async Task Handle_DuplicateSKU_ThrowsValidationExceptionWithLogging()
    {
        // Arrange: Create and save an existing product in the database with a specific SKU
        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Product",
            Brand = "Existing Brand",
            Sku = "DUP-SKU-001",
            Category = ProductCategory.Electronics,
            Price = 99.99m,
            ReleaseDate = DateTime.UtcNow,
            ImageUrl = "https://example.com/product.jpg",
            IsAvailable = true,
            StockQuantity = 10,
            CreatedAt = DateTime.UtcNow
        };
        _context.Products.Add(existingProduct);
        await _context.SaveChangesAsync();

        // Arrange: Create a new request with the same SKU
        var request = new CreateProductCommand
        {
            Name = "New Product",
            Brand = "New Brand",
            Sku = "DUP-SKU-001",
            Category = (int)ProductCategory.Electronics,
            Price = 149.99m,
            ReleaseDate = DateTime.UtcNow,
            ImageUrl = "https://example.com/newproduct.jpg",
            StockQuantity = 20
        };

        // Act & Assert: Verify a ValidationException is thrown
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));

        // Assert: Check the exception message contains "already exists"
        Assert.Contains("already exists", exception.Message);

        // Verify Logging: ProductValidationFailed log called once
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                ProductLogEvents.ProductValidationFailed,
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test 3: Verify Home product request applies discount and conditional mapping.
    /// Checks CategoryDisplayName ("Home & Garden"), ImageUrl is null, and Price has 10% discount.
    /// </summary>
    [Fact]
    public async Task Handle_HomeProductRequest_AppliesDiscountAndConditionalMapping()
    {
        // Arrange: Create a valid Home product request (without ImageUrl to test conditional filtering)
        var request = new CreateProductCommand
        {
            Name = "Office Chair",
            Brand = "Furniture Plus",
            Sku = "HOME-OC-001",
            Category = (int)ProductCategory.Home,
            Price = 250.00m, // Will have 10% discount applied: 250 * 0.9 = 225
            ReleaseDate = DateTime.UtcNow.AddDays(-45), // 45 days old (not New Release)
            ImageUrl = null, // No image URL
            StockQuantity = 15
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert: Check CategoryDisplayName ("Home & Garden")
        Assert.Equal(ProductConstants.CategoryDisplayHome, result.CategoryDisplayName);

        // Assert: Check ImageUrl is null (conditional filtering for Home category)
        Assert.Null(result.ImageUrl);

        // Assert: Check that the Price has a 10% discount applied (250 * 0.9 = 225.00)
        decimal expectedDiscountedPrice = 250.00m * ProductConstants.HomeDiscountMultiplier;
        Assert.Equal(expectedDiscountedPrice, result.Price);
        Assert.Equal(225.00m, result.Price);

        // Additional assertions for Home category logic
        Assert.True(result.IsAvailable);
        Assert.Equal(15, result.StockQuantity);
    }

    /// <summary>
    /// Ensure proper disposal of context and cache resources.
    /// </summary>
    public void Dispose()
    {
        _context?.Dispose();
        _cache?.Dispose();
    }
}
