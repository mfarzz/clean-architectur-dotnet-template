using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;

namespace MyCleanApi.WebApi.IntegrationTests;

public class ApiDocumentationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ApiDocumentationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task OpenApi_ShouldDescribeBearerAuth_WhileKeepingSampleEndpointsPublic()
    {
        using var client = _factory.CreateClient();
        using var document = JsonDocument.Parse(await client.GetStringAsync("/openapi/v1.json"));
        var root = document.RootElement;
        var bearer = root.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer");
        bearer.GetProperty("type").GetString().Should().Be("http");
        bearer.GetProperty("scheme").GetString().Should().Be("bearer");

        foreach (var path in root.GetProperty("paths").EnumerateObject())
        {
            foreach (var operation in path.Value.EnumerateObject())
            {
                var requirements = operation.Value.GetProperty("security").EnumerateArray().ToArray();
                requirements.Should().Contain(requirement => requirement.EnumerateObject().Any(property => property.Name == "Bearer"));
                requirements.Should().Contain(requirement => !requirement.EnumerateObject().Any());
            }
        }
    }

    [Theory]
    [InlineData("/swagger/index.html", "Swagger UI")]
    [InlineData("/swagger-ui/custom.css", "prefers-color-scheme: dark")]
    [InlineData("/swagger-ui/custom.js", "Documentation theme")]
    public async Task DocumentationAssets_ShouldBeAvailableInDevelopment(string path, string expected)
    {
        using var client = _factory.CreateClient();
        var content = await client.GetStringAsync(path);
        content.Should().Contain(expected);
    }

    [Theory]
    [InlineData("/swagger/index.html")]
    [InlineData("/openapi/v1.json")]
    [InlineData("/swagger-ui/custom.css")]
    [InlineData("/swagger-ui/custom.js")]
    public async Task Documentation_ShouldNotBeExposedInProduction(string path)
    {
        using var factory = _factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        using var client = factory.CreateClient();
        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
