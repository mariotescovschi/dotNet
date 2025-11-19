using Microsoft.Extensions.Logging;
using Product_Management_API.Constants;
using Product_Management_API.Metrics;

namespace Product_Management_API.Extensions;

public static class LoggingExtensions
{
    private const string OperationIdProperty = "OperationId";
    private const string ProductNameProperty = "ProductName";
    private const string SKUProperty = "SKU";
    private const string CategoryProperty = "Category";
    private const string SuccessProperty = "Success";
    private const string ErrorReasonProperty = "ErrorReason";
    private const string ValidationDurationMsProperty = "ValidationDurationMs";
    private const string DatabaseSaveDurationMsProperty = "DatabaseSaveDurationMs";
    private const string TotalDurationMsProperty = "TotalDurationMs";

    public static void LogProductCreationMetrics(this ILogger logger, ProductCreationMetrics metrics)
    {
        var state = new Dictionary<string, object>
        {
            { OperationIdProperty, metrics.OperationId },
            { ProductNameProperty, metrics.ProductName },
            { SKUProperty, metrics.SKU },
            { CategoryProperty, metrics.Category.ToString() },
            { SuccessProperty, metrics.Success },
            { ValidationDurationMsProperty, metrics.ValidationDuration.TotalMilliseconds },
            { DatabaseSaveDurationMsProperty, metrics.DatabaseSaveDuration.TotalMilliseconds },
            { TotalDurationMsProperty, metrics.TotalDuration.TotalMilliseconds }
        };

        if (!string.IsNullOrWhiteSpace(metrics.ErrorReason))
        {
            state[ErrorReasonProperty] = metrics.ErrorReason;
        }

        logger.Log(
            metrics.Success ? LogLevel.Information : LogLevel.Error,
            new EventId(ProductLogEvents.ProductCreationCompleted),
            state,
            null,
            (s, ex) => FormatProductCreationMessage(metrics));
    }

    private static string FormatProductCreationMessage(ProductCreationMetrics metrics)
    {
        var status = metrics.Success ? "succeeded" : "failed";
        var errorMessage = string.IsNullOrWhiteSpace(metrics.ErrorReason)
            ? string.Empty
            : $" Reason: {metrics.ErrorReason}";

        return $"Product creation {status}. Name: {metrics.ProductName}, SKU: {metrics.SKU}, " +
               $"Category: {metrics.Category}, ValidationDuration: {metrics.ValidationDuration.TotalMilliseconds}ms, " +
               $"DatabaseSaveDuration: {metrics.DatabaseSaveDuration.TotalMilliseconds}ms, " +
               $"TotalDuration: {metrics.TotalDuration.TotalMilliseconds}ms{errorMessage}";
    }
}