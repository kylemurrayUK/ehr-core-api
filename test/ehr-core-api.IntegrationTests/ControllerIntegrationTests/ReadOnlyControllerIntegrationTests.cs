using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;


namespace ehr_core_api.IntegrationTests;

public class ReadOnlyControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ReadOnlyControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        factory.SeedDatabase();
        _client = factory.CreateClient();
    }

     [Fact]
     public async Task ListAppointments_Return200WithAppointmentList()
    {

         //Act
        var response = await _client.GetAsync("/api/Appointment/ListAppointments");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}