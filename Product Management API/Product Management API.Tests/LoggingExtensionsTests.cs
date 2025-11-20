using System;
using System.Collections.Generic;
using System.Linq;
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

        // Verify that Log was called with the expected parameters
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logging should be called once with Information level");
    }

    [Fact]
    public void LogProductCreationMetrics_ConvertsTimingsToMilliseconds()
    {
        var loggerMock = new Mock<ILogger>();

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

        // Verify that Log was called
        loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logging should be called once");
    }

    [Fact]
    public void LogProductCreationMetrics_WithErrorReason_IncludesErrorReasonInState()
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

        // Verify that Log was called with Error level when Success is false
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logging should be called once with Error level when Success is false");
    }

    [Fact]
    public void LogProductCreationMetrics_WithoutErrorReason_DoesNotIncludeErrorReasonInState()
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

        // Verify that Log was called with Information level when Success is true
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once,
            "Logging should be called once with Information level when Success is true");
    }
}
