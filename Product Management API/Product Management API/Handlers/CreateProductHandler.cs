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
using Product_Management_API.Extensions;
using Product_Management_API.Metrics;

namespace Product_Management_API.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductProfileDto>
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        ApplicationContext context,
        IMapper mapper,
        IMemoryCache cache,
        ILogger<CreateProductHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProductProfileDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var operationId = GenerateOperationId();
        var operationStartTime = DateTime.UtcNow;

        using (_logger.BeginScope(new Dictionary<string, object> { { ProductConstants.OperationIdKey, operationId } }))
        {
            try
            {
                _logger.LogInformation(
                    ProductLogEvents.ProductCreationStarted,
                    ProductConstants.ProductCreationStartedMessage,
                    request.Name,
                    request.Brand,
                    request.Sku,
                    request.Category);

                var validationStartTime = DateTime.UtcNow;

                var skuExists = await _context.Products.AnyAsync(p => p.Sku == request.Sku, cancellationToken);
                var skuValidationDuration = DateTime.UtcNow - validationStartTime;

                _logger.LogInformation(
                    ProductLogEvents.SKUValidationPerformed,
                    ProductConstants.SkuValidationCompletedMessage,
                    request.Sku,
                    skuExists,
                    skuValidationDuration.TotalMilliseconds);

                if (skuExists)
                {
                    _logger.LogError(
                        ProductLogEvents.ProductValidationFailed,
                        ProductConstants.SkuValidationFailedMessage,
                        request.Sku);

                    throw new ValidationException(
                        $"A product with SKU '{request.Sku}' already exists. SKU must be unique.");
                }

                var stockValidationStartTime = DateTime.UtcNow;
                var isValidStock = request.StockQuantity >= ProductConstants.ZeroStock;
                var stockValidationDuration = DateTime.UtcNow - stockValidationStartTime;

                _logger.LogInformation(
                    ProductLogEvents.StockValidationPerformed,
                    ProductConstants.StockValidationCompletedMessage,
                    request.StockQuantity,
                    isValidStock,
                    stockValidationDuration.TotalMilliseconds);

                var totalValidationDuration = DateTime.UtcNow - validationStartTime;

                var createRequest = new CreateProductProfileRequest
                {
                    Name = request.Name,
                    Brand = request.Brand,
                    Sku = request.Sku,
                    Category = (ProductCategory)request.Category,
                    Price = request.Price,
                    ReleaseDate = request.ReleaseDate,
                    ImageUrl = request.ImageUrl,
                    StockQuantity = request.StockQuantity
                };

                var product = _mapper.Map<Product>(createRequest);

                var dbOperationStartTime = DateTime.UtcNow;

                _logger.LogInformation(
                    ProductLogEvents.DatabaseOperationStarted,
                    ProductConstants.DatabaseSaveStartedMessage,
                    product.Name);

                _context.Products.Add(product);
                await _context.SaveChangesAsync(cancellationToken);

                var dbOperationDuration = DateTime.UtcNow - dbOperationStartTime;

                _logger.LogInformation(
                    ProductLogEvents.DatabaseOperationCompleted,
                    ProductConstants.DatabaseSaveCompletedMessage,
                    product.Id,
                    dbOperationDuration.TotalMilliseconds);

                var cacheOperationStartTime = DateTime.UtcNow;

                _cache.Remove(ProductConstants.AllProductsCacheKey);

                var cacheOperationDuration = DateTime.UtcNow - cacheOperationStartTime;

                _logger.LogInformation(
                    ProductLogEvents.CacheOperationPerformed,
                    ProductConstants.CacheInvalidationCompletedMessage,
                    ProductConstants.AllProductsCacheKey,
                    cacheOperationDuration.TotalMilliseconds);

                var totalDuration = DateTime.UtcNow - operationStartTime;

                var metrics = new ProductCreationMetrics(
                    OperationId: operationId,
                    ProductName: product.Name,
                    SKU: product.Sku,
                    Category: product.Category,
                    ValidationDuration: totalValidationDuration,
                    DatabaseSaveDuration: dbOperationDuration,
                    TotalDuration: totalDuration,
                    Success: true);

                _logger.LogProductCreationMetrics(metrics);

                return _mapper.Map<ProductProfileDto>(product);
            }
            catch (ValidationException ex)
            {
                var totalDuration = DateTime.UtcNow - operationStartTime;

                var errorMetrics = new ProductCreationMetrics(
                    OperationId: operationId,
                    ProductName: request.Name,
                    SKU: request.Sku,
                    Category: (ProductCategory)request.Category,
                    ValidationDuration: TimeSpan.Zero,
                    DatabaseSaveDuration: TimeSpan.Zero,
                    TotalDuration: totalDuration,
                    Success: false,
                    ErrorReason: ex.Message);

                _logger.LogProductCreationMetrics(errorMetrics);

                throw;
            }
            catch (Exception ex)
            {
                var totalDuration = DateTime.UtcNow - operationStartTime;

                _logger.LogError(
                    ProductLogEvents.ProductValidationFailed,
                    ProductConstants.UnexpectedErrorMessage,
                    request.Sku,
                    ex.Message);

                var errorMetrics = new ProductCreationMetrics(
                    OperationId: operationId,
                    ProductName: request.Name,
                    SKU: request.Sku,
                    Category: (ProductCategory)request.Category,
                    ValidationDuration: TimeSpan.Zero,
                    DatabaseSaveDuration: TimeSpan.Zero,
                    TotalDuration: totalDuration,
                    Success: false,
                    ErrorReason: ex.Message);

                _logger.LogProductCreationMetrics(errorMetrics);

                throw;
            }
        }
    }

    private static string GenerateOperationId()
    {
        var random = new Random();
        return new string(Enumerable.Range(0, ProductConstants.OperationIdLength)
            .Select(_ =>
                ProductConstants.OperationIdCharacters[random.Next(ProductConstants.OperationIdCharacters.Length)])
            .ToArray());
    }
}