using EHRCoreAPI.Data;
using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories.Implementations;

namespace ehr_core_api.IntegrationTests;

[Collection("Integration tests")]
public class WriteRepositoryIntegrationTests : IDisposable
{
    public readonly AppointmentRepository _appointmentRepository;
    public readonly TestDatabaseFixture _fixture;
    public readonly ApiDbContext _context;
    public  readonly (IReadOnlyList<Patient> patients, IReadOnlyList<Clinician> clinicians, IReadOnlyList<Appointment> appointments) SeededData;

    public WriteRepositoryIntegrationTests()
    {
        _fixture = new TestDatabaseFixture(); 
        var context = _fixture.CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();


        SeededData = TestSeedData.SeedTestData(context);
        _appointmentRepository = new AppointmentRepository(context);
        _context = context;
    }

    [Fact]
    public async Task AddAndSaveAppointmentAsync_CreateAppointment()
    {
        //Arrange
        Appointment testAppointment = new Appointment
        {
            PatientId = SeededData.patients[0].Id,
            Department = "Oncology",
            ClinicianId = SeededData.clinicians[1].Id,
            Status = AppointmentStatus.Pending,
            AppointmentTime = new DateTime(2026, 8, 1, 23, 19, 0)
         };

         //Act
        await _appointmentRepository.AddAndSaveAppointmentAsync(testAppointment);
        
         //Assert
        Assert.NotEqual(0, testAppointment.Id); // Id been set so not default int value (0)
        
        //Create fresh context to prevent identity mapping keeping the same object
        using var testContext = _fixture.CreateContext();
        AppointmentRepository _assertAppointmentRepository = new AppointmentRepository(testContext);
        
        Appointment? assertAppointment = await _assertAppointmentRepository.GetAppointmentAsync(testAppointment.Id);
        Assert.NotNull(assertAppointment);
        Assert.Equal(SeededData.patients[0].Id, assertAppointment.PatientId);
        Assert.Equal(SeededData.clinicians[1].Id, assertAppointment.ClinicianId);
    }

    [Fact]
    public async Task UpdateStatus_UpdatingAppointmentStatus()
    {
        // Arrange 
        AppointmentStatus statusToUpdateTo = AppointmentStatus.Completed;
        AppointmentStatus originalAppointmentStatus = SeededData.appointments[0].Status;
        //Act
        await _appointmentRepository.UpdateStatus(SeededData.appointments[0], statusToUpdateTo);

        //Assert
        //Create fresh context to prevent identity mapping keeping the same object
        using var testContext = _fixture.CreateContext();
        AppointmentRepository _assertAppointmentRepository = new AppointmentRepository(testContext);

        Appointment? assertAppointment = await _assertAppointmentRepository.GetAppointmentAsync(SeededData.appointments[0].Id);
        Assert.NotNull(assertAppointment);
        Assert.NotEqual(originalAppointmentStatus, assertAppointment.Status);
        Assert.Equal(statusToUpdateTo, assertAppointment.Status);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

}