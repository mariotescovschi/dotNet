using Product_Management_API.Enums;

namespace Product_Management_API.Metrics;

public record ProductCreationMetrics(
    string OperationId,
    string ProductName,
    string SKU,
    ProductCategory Category,
    TimeSpan ValidationDuration,
    TimeSpan DatabaseSaveDuration,
    TimeSpan TotalDuration,
    bool Success,
    string? ErrorReason = null);