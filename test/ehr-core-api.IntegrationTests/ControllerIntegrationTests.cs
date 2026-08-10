using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc.Testing;


namespace ehr_core_api.IntegrationTests;

public class ControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        factory.SeedDatabase();
        _client = factory.CreateClient();
    }
}