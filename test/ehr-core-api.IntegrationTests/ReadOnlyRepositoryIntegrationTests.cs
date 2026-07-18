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
    public async Task GetAllAppointments_ReturnListOfAllAppointments()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);

        // Act
        var appointments = await appointmentRepo.GetAllAppointmentsAsync();

        // Assert
        Assert.All(appointments, a => 
        {
            Assert.NotNull(a.Patient);
            Assert.NotNull(a.Clinician);
        });
        Assert.Equal(Fixture.SeededData.appointments.Count, appointments.Count);
    }
}
