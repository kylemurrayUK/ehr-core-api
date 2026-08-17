using EHRCoreAPI.Dtos;
using EHRCoreAPI.Dtos.Output;
using EHRCoreAPI.Models;
using System.Net;


namespace ehr_core_api.IntegrationTests;

public class WriteControllerIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public WriteControllerIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _factory.SeedDatabase();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateAppointment_InvalidModel_Return400BadRequest()
    {
        //Arrange
        var testIncompleteAppointment = new CreateAppointmentDTO()
        {
            PatientId = null,
            ClinicianId = null
        };
        
        //Act
        var response = await _client.PostAsJsonAsync("api/Appointment/CreateAppointment", testIncompleteAppointment);
        var responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Patient Id is required.", responseBody);
        Assert.Contains("Clinician Id name is required.", responseBody);
    }

    [Fact]
    public async Task CreateAppointment_PatientIdNotFound_Return400BadRequest()
    {
        //Arrange
        var testIncorrectPatientIdAppointment = new CreateAppointmentDTO()
        {
            PatientId = 99999,
            ClinicianId = _factory.SeededData.clinicians[0].Id,
            Department = "TestDepartment",
            AppointmentTime = new DateTime(2026, 8, 18, 10, 0, 0)
        };
        
        //Act
        var response = await _client.PostAsJsonAsync("api/Appointment/CreateAppointment", testIncorrectPatientIdAppointment);
        var responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Patient with this ID does not exist.", responseBody);
    }

    [Fact]
    public async Task CreateAppointment_ValidAppointment_Return201WithAppointmentAndLocation()
    {
        //Arrange
        var testCorrectAppointment = new CreateAppointmentDTO()
        {
            PatientId = _factory.SeededData.patients[0].Id,
            ClinicianId = _factory.SeededData.clinicians[0].Id,
            Department = "TestDepartment",
            AppointmentTime = new DateTime(2026, 8, 18, 10, 0, 0)
        };
        
        //Act
        var response = await _client.PostAsJsonAsync("api/Appointment/CreateAppointment", testCorrectAppointment);
        var responseLocation = response.Headers.Location;
        var responseBody = await response.Content.ReadFromJsonAsync<ReturnAppointmentDTO>();

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.NotNull(responseBody);
        Assert.NotEqual(0, responseBody.Id);
        Assert.Equal(_factory.SeededData.patients[0].Id, responseBody.Patient.Id);
        Assert.Equal(_factory.SeededData.clinicians[0].Id, responseBody.Clinician.Id);

        Assert.NotNull(responseLocation);
        var responseLocationResponse = await _client.GetAsync(responseLocation);
        var appointmentAtResponseLocation = await responseLocationResponse.Content.ReadFromJsonAsync<ReturnAppointmentDTO>();
        Assert.NotNull(appointmentAtResponseLocation);
        Assert.Equal(responseBody.Id, appointmentAtResponseLocation.Id);
    }

    public void Dispose()
    {
        if(_factory.Connection != null)
            _factory.Connection.Dispose();

        _factory.Dispose();
    }
}