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

    [Fact]
    public async Task GetAppointment_SearchingForValidAppointment_ReturnAppointment()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        Appointment appointmentSearchedFor = Fixture.SeededData.appointments[1];
        
        // Act
        var appointment = await appointmentRepo.GetAppointmentAsync(appointmentSearchedFor.Id);

        // Assert
        Assert.NotNull(appointment);
        Assert.Equal(Fixture.SeededData.appointments[1].Id, appointment.Id);
    }

    [Fact]
    public async Task GetAppointmentWithDetails_SearchingForValidAppointment_ReturnAppointmentWithDetails()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        Appointment appointmentSearchedFor = Fixture.SeededData.appointments[0];
        
        // Act
        var appointment = await appointmentRepo.GetAppointmentWithDetailsAsync(appointmentSearchedFor.Id);

        // Assert
        Assert.NotNull(appointment);
        Assert.NotNull(appointment.Clinician);
        Assert.NotNull(appointment.Patient);
        Assert.Equal(Fixture.SeededData.appointments[0].PatientId, appointment.Patient.Id);
        Assert.Equal(Fixture.SeededData.appointments[0].ClinicianId, appointment.Clinician.Id);
    }

    // GetAppointmentBy Integration Tests
    [Fact]
    public async Task GetAppointmentsBy_SearchingOnPatientId_ReturnAppointmentsWithMatchingPatient()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        Patient patientSearchedFor = Fixture.SeededData.patients[0];
        var patientTestFilter = new FilterParameters(patientId: patientSearchedFor.Id,null, null, null, null, null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(patientTestFilter);

        // Assert
        Assert.Equal(2, appointments.Count);
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
    }

    [Theory]
    [InlineData("Dec")]
    [InlineData("hone")]
    public async Task GetAppointmentsBy_SearchingOnPatient_ReturnAppointmentWithMatchingPatientName(string partialSearch)
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        var partialPatientNameTestFilter = new FilterParameters(null, null, null, patientName: partialSearch, null, null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(partialPatientNameTestFilter);

        // Assert
        var appointment = Assert.Single(appointments);
        Assert.Equal(Fixture.SeededData.patients[1].Id, appointment.PatientId);

    }

    [Fact]
    public async Task GetAppointmentsBy_SearchingOnClinicianWithMultipleReturns_ReturnTwoAppointments()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        var matchingClinicians = new List<int>
        {
            Fixture.SeededData.clinicians[1].Id,
            Fixture.SeededData.clinicians[2].Id
        };
        var clinicianNameTestFilter = new FilterParameters(null, null, null, null , clinicianName: "Murray", null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(clinicianNameTestFilter);

        // Assert
        Assert.Equal(2, appointments.Count);
        foreach (int clinicianId in matchingClinicians)
        {
            Assert.Contains(appointments, a => a.ClinicianId == clinicianId);
        }
    }

    [Fact]
    public async Task GetAppointmentsBy_SearchingOnStatus_ReturnAppointmentsWithMatchingStatus()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        AppointmentStatus statusSearchedFor = AppointmentStatus.Completed;
        var statusTestFilter = new FilterParameters(null, null, null , null, null, status: statusSearchedFor);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(statusTestFilter);

        // Assert
        Assert.Equal(2, appointments.Count);
    }


    // If "Pharmacology" or "AppointmentStatus.Completed" are searched for individually they get two 
    // appointments each but together only one.
    [Fact]
    public async Task GetAppointmentsBy_SearchingOnMultipleFilters_ReturnAppointmentsWithMatchingParameters()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        AppointmentStatus statusSearchedFor = AppointmentStatus.Completed;
        string departmentSearchedFor = "Pharmacology";
        var multipleFilter = new FilterParameters(null, null, department: departmentSearchedFor , null, null , status: statusSearchedFor);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(multipleFilter);

        // Assert
        var appointment = Assert.Single(appointments);
        Assert.Equal(Fixture.SeededData.patients[0].Id, appointment.PatientId);
    }

    [Fact]
    public async Task GetAppointmentsBy_SearchingOnGarbageValueFilter_ReturnEmptyList()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var appointmentRepo = new AppointmentRepository(context);
        var gargabePatientIdFilter = new FilterParameters(patientId: 99999, null, null , null, null , null);
        
        // Act
        var appointments = await appointmentRepo.GetAppointmentByAsync(gargabePatientIdFilter);

        // Assert
        Assert.Empty(appointments);
    }
}
