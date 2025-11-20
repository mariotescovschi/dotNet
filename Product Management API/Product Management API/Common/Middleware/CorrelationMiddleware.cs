namespace Product_Management_API.Middleware;

/// <summary>
/// Middleware that adds correlation IDs to HTTP requests and responses.
/// This enables tracking of requests across the application.
/// </summary>
public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Get or create correlation ID
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existingId)
            ? existingId.ToString()
            : Guid.NewGuid().ToString();

        // Add correlation ID to response headers
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // Log the correlation ID
        _logger.LogInformation($"Request started with Correlation ID: {correlationId}");

        // Store in context items for use in handlers
        context.Items["CorrelationId"] = correlationId;

        await _next(context);

        _logger.LogInformation($"Request completed with Correlation ID: {correlationId}");
    }
}
