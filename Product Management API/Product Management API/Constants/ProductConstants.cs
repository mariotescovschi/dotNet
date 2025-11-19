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

    // Validation Rules - Name
    public const int NameMinLength = 1;
    public const int NameMaxLength = 200;
    public const string NameRequiredMessage = "Product name is required";
    public const string NameLengthMessage = "Product name must be between {0} and {1} characters";
    public const string NameInappropriateContentMessage = "Product name contains inappropriate content";

    // Validation Rules - Brand
    public const int BrandMinLength = 2;
    public const int BrandMaxLength = 100;
    public const string BrandRequiredMessage = "Brand name is required";
    public const string BrandLengthMessage = "Brand name must be between {0} and {1} characters";

    public const string BrandInvalidCharactersMessage =
        "Brand name contains invalid characters. Only letters, spaces, hyphens, apostrophes, dots, and numbers are allowed";

    public const string BrandMinLengthClothingMessage = "Brand name must be at least 3 characters";
    public const string BrandNamePattern = @"^[a-zA-Z0-9\s\-'.]+$";

    // Validation Rules - SKU
    public const int SkuMinLength = 5;
    public const int SkuMaxLength = 20;
    public const string SkuRequiredMessage = "SKU is required";
    public const string SkuFormatMessage = "SKU must be alphanumeric with hyphens, between {0} and {1} characters";
    public const string SkuUniqueMessage = "SKU must be unique in the system";
    public const string SkuPattern = @"^[a-zA-Z0-9\-]{5,20}$";

    // Validation Rules - Category
    public const string CategoryValidMessage = "Category must be a valid enum value";

    // Validation Rules - Price
    public const decimal MaxPrice = 10000m;
    public const decimal MinPriceValue = 0.01m;
    public const decimal ElectronicsMinPrice = 50m;
    public const decimal HighValueProductThreshold = 500m;
    public const decimal HomeMaxPrice = 200m;
    public const decimal ExpensiveProductThreshold = 100m;
    public const string PriceGreaterThanZeroMessage = "Price must be greater than 0";
    public const string PriceMaxMessage = "Price must be less than {0:C2}";
    public const string ElectronicsMinPriceMessage = "Electronics products must have a minimum price of {0:C2}";
    public const string HomeMaxPriceMessage = "Home products must not exceed $200.00";

    // Validation Rules - ReleaseDate
    public const int MinYear = 1900;
    public const int ElectronicsLastYears = 5;
    public const string ReleaseDateFutureMessage = "Release date cannot be in the future";
    public const string ReleaseDateMinYearMessage = "Release date cannot be before year {0}";

    public const string ElectronicsReleaseDateMessage =
        "Electronics products must be released within the last {0} years";

    // Validation Rules - StockQuantity
    public const int MaxStockQuantity = 100000;
    public const int ExpensiveProductMaxStock = 20;
    public const int HighValueProductMaxStock = 10;
    public const string StockNegativeMessage = "Stock quantity cannot be negative";
    public const string StockMaxMessage = "Stock quantity cannot exceed {0}";
    public const string ExpensiveProductStockMessage = "Expensive products (>{0}) must have limited stock (≤{1} units)";

    // Validation Rules - ImageUrl
    public const string ImageUrlValidMessage =
        "Image URL must be valid (HTTP/HTTPS protocol and must end with valid image extensions: .jpg, .jpeg, .png, .gif, .webp)";

    public const string ImageExtensionsPattern = @"\.(jpg|jpeg|png|gif|webp)$";

    // Business Rules - Daily Product Limit
    public const int MaxDailyProducts = 500;
    public const string DailyProductLimitMessage = "Daily product addition limit reached. Products added today: {0}";

    // Business Rules - High Value Product
    public const int HighValueProductMaxStockLimit = 10;

    // Inappropriate Words List
    public static readonly string[] InappropriateWords =
    {
        "offensive", "inappropriate", "banned", "blocked"
    };

    // Restricted Home Words
    public static readonly string[] RestrictedHomeWords =
    {
        "danger", "hazard", "toxic", "explosive"
    };

    // Technology Keywords for Electronics
    public static readonly string[] TechnologyKeywords =
    {
        "tech", "gadget", "device", "electronics", "software", "hardware"
    };

    public const string ElectronicsTechnologyKeywordsMessage =
        "Electronics product name must contain technology-related keywords";

    public const string HomeInappropriateContentMessage =
        "Home product name contains inappropriate content for home category";

    public const string NameUniquePerBrandMessage = "Product name must be unique for the same brand";
    public const string BusinessRulesFailedMessage = "Product request failed business validation";

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

    public const string SkuValidationCompletedMessage =
        "SKU validation completed for SKU: {SKU}. Exists: {SKUExists}, Duration: {ValidationDurationMs}ms";

    public const string SkuValidationFailedMessage = "SKU validation failed. Duplicate SKU detected: {SKU}";
    public const string SkuAlreadyExistsMessage = "A product with SKU '{SKU}' already exists. SKU must be unique.";

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

    // Entity Property Max Lengths
    public const int NameMaxLengthDb = 255;
    public const int BrandMaxLengthDb = 255;
    public const int SkuMaxLengthDb = 50;
    public const int ImageUrlMaxLengthDb = 2048;
}