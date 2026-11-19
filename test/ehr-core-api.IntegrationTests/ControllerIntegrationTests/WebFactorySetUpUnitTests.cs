using System.Net;
using EHRCoreAPI.Configuration;
using EHRCoreAPI.Models;
using Microsoft.Extensions.Options;

public class WebFactorySetUpUnitTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public WebFactorySetUpUnitTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void TestFactory_UsesTestJwtSettings()
    {
        // Arrange & Act
        var settings = _factory.Services
            .GetRequiredService<IOptions<JwtSettings>>()
            .Value;

        // Assert
        Assert.Equal(Convert.ToBase64String(new byte[32]), settings.Key);
        Assert.Equal("Test Issuer", settings.Issuer);
    }

    [Fact]
    public async Task CustomWebApplicationFactory_AdminRoleRequest_Returns200()
    {
        // Arrange
        _factory.SeedDatabase();
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Admin);

        var response = await client.GetAsync("/api/Appointment/ListAppointments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}