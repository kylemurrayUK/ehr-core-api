using System.Net;
using EHRCoreAPI.Dtos;
using EHRCoreAPI.Dtos.Output;
using RestSharp;

namespace ehr_core_api.SmokeTests;

[Trait("Category", "Smoke")]
public class SmokeTests : IClassFixture<SmokeTestFixture>
{

    private readonly SmokeTestFixture _fixture;

    public SmokeTests(SmokeTestFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task LoginAsync_LoginWithValidCredentials_Returns200WithToken()
    {
        // Arrange
        using var unauthenticatedClient = new RestClient(SmokeTestFixture.BaseUrl);
        var validLoginCredentials = new LoginRequestDTO()
        {
            Email = SmokeTestFixture.ValidEmailForLogin,
            Password = SmokeTestFixture.ValidPasswordForLogin
        };
        var request = new RestRequest("/api/Auth/Login");
        request.AddJsonBody(validLoginCredentials);

        var response = await unauthenticatedClient.ExecutePostAsync<LoginResponseDTO>(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotEmpty(response.Data.Token);
    }

    [Fact]
    public async Task GetAllAppointments_APIRunning_Returns200WithList()
    {
        //Arrange
        var request = new RestRequest("/api/Appointment/ListAppointments");

        //Act
        var response = await _fixture.Client.ExecuteAsync<List<ReturnAppointmentDTO>>(request);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task CreateAppointment_APIRunning_Returnss201WithAppointment()
    {
        //Arrange
        var testAppointmentToCreate = new CreateAppointmentDTO
        {
            // I'm asssuming that a clinician and a patient will have been created here.
            // My seeder guarantees it. It would have had to be manually deleted for this not to be true.
            // It could be clearer that the test data has broke but for now its a reasonable assumption. 
            PatientId = 1,
            Department = "Test Department",
            ClinicianId = 1,
            AppointmentTime = new DateTime(2011, 11, 11, 11, 11, 11)
        };
        var request = new RestRequest("/api/Appointment/CreateAppointment", Method.Post);
        request.AddJsonBody(testAppointmentToCreate);

        //Act
        var response = await _fixture.Client.ExecutePostAsync<ReturnAppointmentDTO>(request);
        
        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotEqual(0, response.Data.Id);
    }

    [Fact]
    public async Task GetAppointment_APIRunningButIncorrectAppointment_Return404WithMessage()
    {
        //Arrange
        int incorrectAppointmentId = 9999999;
        var request = new RestRequest($"/api/Appointment/GetAppointment/{incorrectAppointmentId}");

        //Act
        var response = await _fixture.Client.ExecuteAsync(request);
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(response.Content);
        Assert.Contains("Appointment not found or you don't have access to it", response.Content);
    }
}
