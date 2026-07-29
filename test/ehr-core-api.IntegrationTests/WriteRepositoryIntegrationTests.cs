using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories.Implementations;

namespace ehr_core_api.IntegrationTests;

public class WriteRepositoryIntegrationTests
{
    public readonly AppointmentRepository _appointmentRepository;
    public readonly TestDatabaseFixture _fixture;
    

    public  (IReadOnlyList<Patient> patients, IReadOnlyList<Clinician> clinicians, IReadOnlyList<Appointment> appointments) SeededData {get;}

    public WriteRepositoryIntegrationTests()
    {
        _fixture = new TestDatabaseFixture(); 
        var context = _fixture.CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        SeededData = TestSeedData.SeedTestData(context);
        _appointmentRepository = new AppointmentRepository(context);
    }

}