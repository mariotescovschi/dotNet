using MediatR;
using Microsoft.EntityFrameworkCore;
using Product_Management_API.Commands;
using Product_Management_API.Data;
using Product_Management_API.DTOs;
using Product_Management_API.Validators;
using Product_Management_API.Mapping;
using Product_Management_API.Middleware;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
// Register both AutoMapper profiles
builder.Services.AddAutoMapper(typeof(ProductMappingProfile), typeof(AdvancedProductMappingProfile));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseInMemoryDatabase("ProductManagementDb"));
builder.Services.AddMemoryCache();
// Register CreateProductProfileValidator as scoped service
builder.Services.AddScoped<CreateProductProfileValidator>();
// Register all validators from assembly containing CreateProductProfileValidator
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// Add Correlation Middleware to HTTP pipeline
app.UseMiddleware<CorrelationMiddleware>();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

// Update endpoint mapping to /products with product-specific documentation
app.MapPost("/products", async (CreateProductProfileRequest request, IMediator mediator) =>
    {
        var command = new CreateProductCommand(request);
        return await mediator.Send(command);
    })
    .WithName("CreateProduct")
    .WithOpenApi()
    .WithDescription("Creates a new product with validation, mapping, and logging")
    .Produces<ProductProfileDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}