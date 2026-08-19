using System.Net;
using EHRCoreAPI.Dtos;
using EHRCoreAPI.Dtos.Output;
using RestSharp;

namespace ehr_core_api.SmokeTests;

public class SmokeTests : IClassFixture<SmokeTestFixture>
{

    private SmokeTestFixture _fixture;

    public SmokeTests(SmokeTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "Smoke")]
    [Fact]
    public async Task GetAllAppointments_APIRunning_Return200WithList()
    {
        //Arrange
        var request = new RestRequest("/api/Appointment/ListAppointments");

        //Act
        var response = await _fixture.Client.ExecuteAsync<List<ReturnAppointmentDTO>>(request);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
    }

    [Trait("Category", "Smoke")]
    [Fact]
    public async Task CreateAppointment_APIRunning_Return201WithAppointment()
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
}
