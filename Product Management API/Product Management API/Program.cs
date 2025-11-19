using MediatR;
using Microsoft.EntityFrameworkCore;
using Product_Management_API.Commands;
using Product_Management_API.Data;
using Product_Management_API.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseInMemoryDatabase("ProductManagementDb"));
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

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

app.MapPost("/api/products", async (CreateProductProfileRequest request, IMediator mediator) =>
    {
        var command = new CreateProductCommand(request);
        return await mediator.Send(command);
    })
    .WithName("CreateProduct")
    .WithOpenApi()
    .Produces<ProductProfileDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}