using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCleanApi.Application.Common.Models;
using MyCleanApi.Application.Features.SampleItems.Commands.CreateSampleItem;
using MyCleanApi.Application.Features.SampleItems.DTOs;
using MyCleanApi.Infrastructure.Persistence;

namespace MyCleanApi.WebApi.IntegrationTests;

public class SampleItemsEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public SampleItemsEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateSampleItem_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var command = new CreateSampleItemCommand("Integration Test Item", "Integration Test Description");

        // Act
        var response = await _client.PostAsJsonAsync("/api/sampleitems", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<SampleItemDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be("Integration Test Item");
    }

    [Fact]
    public async Task CreateSampleItem_WithEmptyTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateSampleItemCommand(string.Empty, "Description");

        // Act
        var response = await _client.PostAsJsonAsync("/api/sampleitems", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
