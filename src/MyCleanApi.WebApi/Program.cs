using MyCleanApi.Application;
using MyCleanApi.Infrastructure;
using MyCleanApi.WebApi.Endpoints;
using MyCleanApi.WebApi.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Setup
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// 2. Application & Infrastructure Layer DI
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. WebApi Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4. HTTP Request Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("../openapi/v1.json", "MyCleanApi v1");
        options.DocumentTitle = "MyCleanApi Swagger UI";
    });

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("MyCleanApi API Documentation")
               .WithTheme(ScalarTheme.Moon);
    });
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

// 5. Map Endpoints
app.MapControllers();
app.MapSampleItemEndpoints();
app.MapHealthChecks("/health");

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
