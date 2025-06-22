using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PsychologistBooking.Application.Dtos;
using PsychologistBooking.IntegrationTests.Helpers;

namespace PsychologistBooking.IntegrationTests;

public class PsychologistApiTests : IClassFixture<FunctionTestFixture>
{
    private readonly HttpClient _client;
    private readonly IServiceProvider _services;

    public PsychologistApiTests(FunctionTestFixture fixture)
    {
        _services = fixture.Services;
        // TODO: Make the base address configurable
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:7001")
        };
    }

    [Fact]
    public async Task GetPsychologists_Returns_All()
    {
        var response = await _client.GetAsync("/api/psychologists");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResultDto<PsychologistDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPsychologists_ByName_Returns_Filtered()
    {
        var response = await _client.GetAsync("/api/psychologists?name=Alice");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResultDto<PsychologistDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().ContainSingle(p => p.Name == "Alice Anderson");
    }

    [Fact]
    public async Task GetPsychologists_ByType_Returns_Filtered()
    {
        var response = await _client.GetAsync("/api/psychologists?type=Clinical");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResultDto<PsychologistDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(p => p.PsychologistType == "Clinical");
    }
}