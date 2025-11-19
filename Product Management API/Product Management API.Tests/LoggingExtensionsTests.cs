using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using Product_Management_API.Constants;
using Product_Management_API.Enums;
using Product_Management_API.Extensions;
using Product_Management_API.Metrics;

namespace Product_Management_API.Tests;

public class LoggingExtensionsTests
{
    [Fact]
    public void LogProductCreationMetrics_WithSuccessfulOperation_LogsInformationLevel()
    {
        var loggerMock = new Mock<ILogger>();
        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.FromMilliseconds(100),
            DatabaseSaveDuration: TimeSpan.FromMilliseconds(200),
            TotalDuration: TimeSpan.FromMilliseconds(300),
            Success: true);

        loggerMock.Object.LogProductCreationMetrics(metrics);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogProductCreationMetrics_WithFailedOperation_LogsErrorLevel()
    {
        var loggerMock = new Mock<ILogger>();
        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.Zero,
            DatabaseSaveDuration: TimeSpan.Zero,
            TotalDuration: TimeSpan.FromMilliseconds(50),
            Success: false,
            ErrorReason: "Duplicate SKU");

        loggerMock.Object.LogProductCreationMetrics(metrics);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogProductCreationMetrics_UsesCorrectEventId()
    {
        var loggerMock = new Mock<ILogger>();
        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.FromMilliseconds(100),
            DatabaseSaveDuration: TimeSpan.FromMilliseconds(200),
            TotalDuration: TimeSpan.FromMilliseconds(300),
            Success: true);

        loggerMock.Object.LogProductCreationMetrics(metrics);

        loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.Is<EventId>(e => e.Id == ProductLogEvents.ProductCreationCompleted),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogProductCreationMetrics_IncludesAllMetricsInState()
    {
        var loggerMock = new Mock<ILogger>();
        var capturedState = default(Dictionary<string, object>);

        loggerMock
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback((LogLevel level, EventId eventId, object state, Exception? ex, Delegate formatter) =>
            {
                if (state is IReadOnlyList<KeyValuePair<string, object>> stateList)
                {
                    capturedState = stateList.ToDictionary(x => x.Key, x => x.Value);
                }
            });

        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.FromMilliseconds(100),
            DatabaseSaveDuration: TimeSpan.FromMilliseconds(200),
            TotalDuration: TimeSpan.FromMilliseconds(300),
            Success: true);

        loggerMock.Object.LogProductCreationMetrics(metrics);

        Assert.NotNull(capturedState);
        Assert.Contains("OperationId", capturedState.Keys);
        Assert.Contains("ProductName", capturedState.Keys);
        Assert.Contains("SKU", capturedState.Keys);
        Assert.Contains("Category", capturedState.Keys);
        Assert.Contains("Success", capturedState.Keys);
        Assert.Contains("ValidationDurationMs", capturedState.Keys);
        Assert.Contains("DatabaseSaveDurationMs", capturedState.Keys);
        Assert.Contains("TotalDurationMs", capturedState.Keys);
    }

    [Fact]
    public void LogProductCreationMetrics_ConvertsTimingsToMilliseconds()
    {
        var loggerMock = new Mock<ILogger>();
        var capturedState = default(Dictionary<string, object>);

        loggerMock
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback((LogLevel level, EventId eventId, object state, Exception? ex, Delegate formatter) =>
            {
                if (state is IReadOnlyList<KeyValuePair<string, object>> stateList)
                {
                    capturedState = stateList.ToDictionary(x => x.Key, x => x.Value);
                }
            });

        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.FromMilliseconds(100),
            DatabaseSaveDuration: TimeSpan.FromMilliseconds(250),
            TotalDuration: TimeSpan.FromMilliseconds(350),
            Success: true);

        loggerMock.Object.LogProductCreationMetrics(metrics);

        Assert.NotNull(capturedState);
        Assert.Equal(100.0, capturedState["ValidationDurationMs"]);
        Assert.Equal(250.0, capturedState["DatabaseSaveDurationMs"]);
        Assert.Equal(350.0, capturedState["TotalDurationMs"]);
    }

    [Fact]
    public void LogProductCreationMetrics_WithErrorReason_IncludesErrorReasonInState()
    {
        var loggerMock = new Mock<ILogger>();
        var capturedState = default(Dictionary<string, object>);

        loggerMock
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback((LogLevel level, EventId eventId, object state, Exception? ex, Delegate formatter) =>
            {
                if (state is IReadOnlyList<KeyValuePair<string, object>> stateList)
                {
                    capturedState = stateList.ToDictionary(x => x.Key, x => x.Value);
                }
            });

        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.Zero,
            DatabaseSaveDuration: TimeSpan.Zero,
            TotalDuration: TimeSpan.FromMilliseconds(50),
            Success: false,
            ErrorReason: "Duplicate SKU");

        loggerMock.Object.LogProductCreationMetrics(metrics);

        Assert.NotNull(capturedState);
        Assert.Contains("ErrorReason", capturedState.Keys);
        Assert.Equal("Duplicate SKU", capturedState["ErrorReason"]);
    }

    [Fact]
    public void LogProductCreationMetrics_WithoutErrorReason_DoesNotIncludeErrorReasonInState()
    {
        var loggerMock = new Mock<ILogger>();
        var capturedState = default(Dictionary<string, object>);

        loggerMock
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback((LogLevel level, EventId eventId, object state, Exception? ex, Delegate formatter) =>
            {
                if (state is IReadOnlyList<KeyValuePair<string, object>> stateList)
                {
                    capturedState = stateList.ToDictionary(x => x.Key, x => x.Value);
                }
            });

        var metrics = new ProductCreationMetrics(
            OperationId: "ABC12345",
            ProductName: "Test Product",
            SKU: "SKU001",
            Category: ProductCategory.Electronics,
            ValidationDuration: TimeSpan.FromMilliseconds(100),
            DatabaseSaveDuration: TimeSpan.FromMilliseconds(200),
            TotalDuration: TimeSpan.FromMilliseconds(300),
            Success: true);

        loggerMock.Object.LogProductCreationMetrics(metrics);

        Assert.NotNull(capturedState);
        Assert.DoesNotContain("ErrorReason", capturedState.Keys);
    }
}

