using EHRCoreAPI.Models;
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
    
    // GetAppointmentBy Integration Tests
    [Fact]
    public async Task GetAppointmentsBy_SearchingOnPatientId_ReturnAppointmentsWithMatchingPatient()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        Patient patientSearchedFor = Fixture.SeededData.patients[2];
        var patientTestFilter = new FilterParameters(patientId: patientSearchedFor.Id,null, null, null, null, null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(patientTestFilter);

        // Assert
        Assert.Equal(2, appointments.Count);
        Assert.All(appointments, a => 
        {
            Assert.Equal( patientSearchedFor.Id, a.PatientId);
        });
    }

    [Fact]
    public async Task GetAppointmentsBy_SearchingOnClinicianId_ReturnAppointmentsWithMatchingClinician()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        Clinician clinicianSearchedFor = Fixture.SeededData.clinicians[3];
        var clinicianTestFilter = new FilterParameters(null, clinicianId: clinicianSearchedFor.Id, null, null, null, null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(clinicianTestFilter);

        // Assert
        Assert.Equal(2, appointments.Count);
        Assert.All(appointments, a => 
        {
            Assert.Equal( clinicianSearchedFor.Id, a.ClinicianId);
        });
    }

        [Fact]
    public async Task GetAppointmentsBy_SearchingOnDepartment_ReturnAppointmentsWithMatchingDepartment()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        string departmentSearchedFor = "Pharmacology";
        var departmentTestFilter = new FilterParameters(null, null, department: departmentSearchedFor, null, null, null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(departmentTestFilter);

        // Assert
        Assert.Equal(2, appointments.Count);
        Assert.All(appointments, a => 
        {
            Assert.Equal(departmentSearchedFor, a.Department);
        });
    }

}
