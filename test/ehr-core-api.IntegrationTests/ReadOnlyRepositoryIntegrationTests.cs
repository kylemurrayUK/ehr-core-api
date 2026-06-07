using EHRCoreAPI.Repositories.Implementations;


namespace ehr_core_api.IntegrationTests;

public class ReadOnlyRepositoryIntegrationTests : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; }
    
    public ReadOnlyRepositoryIntegrationTests(TestDatabaseFixture fixture)
    {
        Fixture = fixture;

    }
    [Fact]
    public async Task GetAllAppointments_RetrieveAllAppointments_ReturnListOfAllAppointments()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);

        // Act
        var appointments = await appointmentRepo.GetAllAppointmentsAsync();

        // Assert
        Assert.Contains(appointments, a => a.Patient != null);
        Assert.Contains(appointments, a => a.Clinician != null);

    }
}
