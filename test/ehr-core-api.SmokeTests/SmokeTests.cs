using System.Net;
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
        //Act
        var request = new RestRequest("/api/Appointment/ListAppointments");
        var response = await _fixture.Client.ExecuteAsync<List<ReturnAppointmentDTO>>(request);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
    }
}
