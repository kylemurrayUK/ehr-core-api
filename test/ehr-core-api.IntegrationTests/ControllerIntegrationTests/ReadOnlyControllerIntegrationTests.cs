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

    [Fact]
    public async Task ListAppointments_ClinicianSearchesAppointments_OnlyCliniciansAppointmentsReturned()
    {
        // Arrange
        int clinicianId = _factory.SeededData.clinicians[3].Id;
        int testCliniciansAmountOfAppointments = _factory.SeededData.appointments.Count(a => a.ClinicianId == clinicianId);
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Clinician, clinicianId);

        // Act
        var response = await client.GetAsync("/api/Appointment/ListAppointments");
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<List<ReturnAppointmentDTO>>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        Assert.NotEmpty(responseBodyAsAppointment);
        Assert.Equal(testCliniciansAmountOfAppointments, responseBodyAsAppointment.Count);
        Assert.All(responseBodyAsAppointment, a => {Assert.Equal(clinicianId,a.Clinician.Id);});
    }
    
    [Fact]
    public async Task GetAppointmentBy_ClinicianFiltersUsingAnotherCliniciansId_ReturnEmptyListWith200()
    {
        // Arrange
        int clinicianIdForLogin = _factory.SeededData.clinicians[0].Id;
        int clinicianIdForSearching = _factory.SeededData.clinicians[3].Id;
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Clinician, clinicianIdForLogin);

        // Act
        var response = await client.GetAsync($"/api/Appointment/GetAppointmentsBy?clinicianId={clinicianIdForSearching}");
        var responseBodyAsAppointment = await response.Content.ReadFromJsonAsync<List<ReturnAppointmentDTO>>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBodyAsAppointment);
        Assert.Empty(responseBodyAsAppointment);
    } 

    [Fact]
    public async Task GetAppointment_SearchingForDifferentClinicianAndInvalidId_Return404AndBothAreEqual()
    {
        // Arrange
        int clinicianIdForLogin = _factory.SeededData.clinicians[0].Id;
        int clinicianIdForSearching = _factory.SeededData.clinicians[3].Id;
        var client = _factory.CreateClientAuthenticatedAs(UserRole.Clinician, clinicianIdForLogin);

        // Act
        var responseForSearchingWrongClinician = await client.GetAsync($"/api/Appointment/GetAppointment/{clinicianIdForSearching}");
        string responseForSearchingWrongClinicianMessage = await responseForSearchingWrongClinician.Content.ReadAsStringAsync();
        
        var responseForSearchingIncorrectId = await client.GetAsync($"/api/Appointment/GetAppointment/{12348765}");
        string responseForSearchingIncorrectIdMessage = await responseForSearchingIncorrectId.Content.ReadAsStringAsync();


        // Assert
        Assert.Equal(HttpStatusCode.NotFound, responseForSearchingWrongClinician.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responseForSearchingIncorrectId.StatusCode);
        Assert.Equal(responseForSearchingWrongClinicianMessage, responseForSearchingIncorrectIdMessage);

    } 
    
}