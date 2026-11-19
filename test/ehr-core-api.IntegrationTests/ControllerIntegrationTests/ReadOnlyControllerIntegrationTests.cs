using EHRCoreAPI.Dtos.Output;
using System.Net;
using EHRCoreAPI.Models;
using System.Net.Http.Headers;


namespace ehr_core_api.IntegrationTests;

public class ReadOnlyControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ReadOnlyControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        factory.SeedDatabase();
    }

     [Fact]
     public async Task ListAppointments_Return200WithAppointmentList()
    {
        // Arrange
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Admin);
        //Act
        var response = await client.GetAsync("/api/Appointment/ListAppointments");
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<List<ReturnAppointmentDTO>>();
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        Assert.Equal(_factory.SeededData.appointments.Count, responseBodyAsAppointment.Count);
        var appointmentToVerify = Assert.Single(responseBodyAsAppointment, a => a.Id == _factory.SeededData.appointments[1].Id);
        Assert.Equal(_factory.SeededData.appointments[1].Clinician.Id, appointmentToVerify.Clinician.Id);
    }

    [Fact]
    public async Task GetAppointment_InvalidId_ReturnNotFoundWithErrorMessage()
    {
        // Arrange
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Admin);
        var testId = 999999;
    
        // Act
        var response = await client.GetAsync($"/api/Appointment/GetAppointment/{testId}");
        var responseBody = await response.Content.ReadAsStringAsync(); 

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Appointment not found", responseBody);
    }

    [Fact]
    public async Task GetAppointment_ValidId_ReturnOkWithAppointmentDTO()
    {
        // Arrange
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Admin);        
        var appointmentUnderTest = _factory.SeededData.appointments[1];
    
        // Act
        var response = await client.GetAsync($"/api/Appointment/GetAppointment/{appointmentUnderTest.Id}");
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<ReturnAppointmentDTO>(); 
  
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        Assert.Equal(appointmentUnderTest.Id, responseBodyAsAppointment.Id);
        Assert.Equal(appointmentUnderTest.Clinician.Id, responseBodyAsAppointment.Clinician.Id);
    }

    [Fact]
    public async Task GetAppointmentBy_NoParameters_ReturnBadRequestWithErrorMessage()
    {
        // Arrange
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Admin);

        // Act
        var response = await client.GetAsync("/api/Appointment/GetAppointmentsBy/");
        var responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("No query included", responseBody);
    }


    [Fact]
    public async Task GetAppointmentBy_ThreeParameters_ReturnOkWithAppropriateAppointments()
    {
        // Arrange
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Admin);
        var testAppointment = _factory.SeededData.appointments[1];

        // Act
        var response = await client.GetAsync($"/api/Appointment/GetAppointmentsBy?patientId={testAppointment.PatientId}&department={testAppointment.Department}&status={testAppointment.Status}");
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<List<ReturnAppointmentDTO>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        var appointmentToAssert = Assert.Single(responseBodyAsAppointment);
        Assert.Equal(testAppointment.PatientId, appointmentToAssert.Patient.Id);
        Assert.Equal(testAppointment.Department, appointmentToAssert.Department);
        Assert.Equal(testAppointment.Status, appointmentToAssert.Status);
    }

    [Fact]
    public async Task ListAppointments_BlankAuthenticationHeader_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Appointment/ListAppointments");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListAppointments_InvalidBearerToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = 
                            new AuthenticationHeaderValue("Bearer", "invalidtokenvalue");

        // Act
        var response = await client.GetAsync("/api/Appointment/ListAppointments");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task ListAppointments_NullIdInToken_Returns403()
    {
        // Arrange
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Clinician);


        // Act
        var response = await client.GetAsync("/api/Appointment/ListAppointments");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
}