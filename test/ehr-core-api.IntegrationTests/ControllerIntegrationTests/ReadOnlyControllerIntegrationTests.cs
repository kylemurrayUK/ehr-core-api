using EHRCoreAPI.Dtos.Output;
using EHRCoreAPI.Mappers;
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
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<List<ReturnAppointmentDTO>>();
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        Assert.Equal(_factory.SeededData.appointments.Count, responseBodyAsAppointment.Count);
        var appointmentToVerify = Assert.Single(responseBodyAsAppointment, a => a.Id == _factory.SeededData.appointments[0].Clinician.Id);
        Assert.Equal(_factory.SeededData.appointments[1].Clinician.Id, appointmentToVerify.Clinician.Id);
    }

    [Fact]
    public async Task GetAppointment_InvalidId_ReturnNotFoundWithErrorMessage()
    {
        // Arrange
        var testId = 999999;
    
        // Act
        var response = await _client.GetAsync($"/api/Appointment/GetAppointment/{testId}");
        var responseBody = await response.Content.ReadAsStringAsync(); 

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains(testId.ToString(), responseBody);
    }

    [Fact]
    public async Task GetAppointment_ValidId_ReturnOkWithAppointmentDTO()
    {
        // Arrange
        var appointmentUnderTest = _factory.SeededData.appointments[1];
    
        // Act
        var response = await _client.GetAsync($"/api/Appointment/GetAppointment/{appointmentUnderTest.Id}");
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<ReturnAppointmentDTO>(); 
  
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        Assert.Equal(appointmentUnderTest.Id, responseBodyAsAppointment.Id);
        Assert.Equal(appointmentUnderTest.Clinician.Id, responseBodyAsAppointment.Clinician.Id);
    }

}