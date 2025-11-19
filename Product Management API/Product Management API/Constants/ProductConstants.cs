namespace Product_Management_API.Constants;

/// <summary>
/// Constants used throughout the Product Management API.
/// Centralized magic values for maintainability and consistency.
/// </summary>
public static class ProductConstants
{
    // Product Age Resolver thresholds (in days)
    public const int NewReleaseThresholdDays = 30;
    public const int MonthsOldThresholdDays = 365;
    public const int YearsOldThresholdDays = 1825; // 5 years

    // Average days calculations
    public const double AverageDaysPerMonth = 30.44;
    public const double AverageDaysPerYear = 365.25;

    // Stock quantity thresholds for availability status
    public const int ZeroStock = 0;
    public const int LastItemStock = 1;
    public const int LimitedStockThreshold = 5;

    // Price discount for Home category
    public const decimal HomeDiscountMultiplier = 0.9m;

    // Currency format
    public const string CurrencyFormat = "C2";

    // Cache keys
    public const string AllProductsCacheKey = "all_products";

    // Availability status messages
    public const string AvailabilityStatusOutOfStock = "Out of Stock";
    public const string AvailabilityStatusUnavailable = "Unavailable";
    public const string AvailabilityStatusLastItem = "Last Item";
    public const string AvailabilityStatusLimitedStock = "Limited Stock";
    public const string AvailabilityStatusInStock = "In Stock";

    // Product age messages
    public const string ProductAgeNewRelease = "New Release";
    public const string ProductAgeClassic = "Classic";

    // Category display names
    public const string CategoryDisplayElectronics = "Electronics & Technology";
    public const string CategoryDisplayClothing = "Clothing & Fashion";
    public const string CategoryDisplayBooks = "Books & Media";
    public const string CategoryDisplayHome = "Home & Garden";
    public const string CategoryDisplayDefault = "Uncategorized";

    // Brand initials placeholder
    public const string BrandInitialsPlaceholder = "?";

    // Logging
    public const string LoggerPrefix = "[CreateProductHandler]";
    public const string LoggerSuccessMessage = "New product created successfully.";
    public const string LoggerIdLabel = "Id";
    public const string LoggerNameLabel = "Name";
    public const string LoggerBrandLabel = "Brand";
    public const string LoggerCategoryLabel = "Category";
    public const string LoggerSkuLabel = "SKU";
    public const string LoggerPriceLabel = "Price";
    public const string LoggerStockQuantityLabel = "Stock Quantity";
    public const string LoggerCreatedAtLabel = "Created At";

    // Operation logging messages
    public const string ProductCreationStartedMessage =
        "Product creation started. Name: {ProductName}, Brand: {Brand}, SKU: {SKU}, Category: {Category}";

    public const string SKUValidationCompletedMessage =
        "SKU validation completed for SKU: {SKU}. Exists: {SKUExists}, Duration: {ValidationDurationMs}ms";

    public const string SKUValidationFailedMessage = "SKU validation failed. Duplicate SKU detected: {SKU}";
    public const string SKUAlreadyExistsMessage = "A product with SKU '{SKU}' already exists. SKU must be unique.";

    public const string StockValidationCompletedMessage =
        "Stock validation completed. Quantity: {StockQuantity}, Valid: {IsValidStock}, Duration: {ValidationDurationMs}ms";

    public const string DatabaseSaveStartedMessage = "Database save operation started for product: {ProductName}";

    public const string DatabaseSaveCompletedMessage =
        "Database save operation completed. ProductId: {ProductId}, Duration: {OperationDurationMs}ms";

    public const string CacheInvalidationCompletedMessage =
        "Cache invalidation completed for key: {CacheKey}, Duration: {OperationDurationMs}ms";

    public const string UnexpectedErrorMessage =
        "Unexpected error during product creation for SKU: {SKU}. Error: {ErrorMessage}";

    // Logging scope and property names
    public const string OperationIdKey = "OperationId";
    public const string OperationIdCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public const int OperationIdLength = 8;
}
