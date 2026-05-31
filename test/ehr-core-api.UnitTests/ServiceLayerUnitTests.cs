using EHRCoreAPI.Repositories;
using EHRCoreAPI;
using EHRCoreAPI.Models;
using EHRCoreAPI.Dtos;
using System.Data.Common;



namespace ehr_core_api.Tests;

public class ServiceLayerUnitTests
{
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
    private readonly Mock<IClinicianRepository> _mockClinicianRepo;
    private readonly Mock<IPatientRepository> _mockPatientRepo;

    private readonly AppointmentService _appointmentService;

    public ServiceLayerUnitTests()
    {
        _mockAppointmentRepo = new Mock<IAppointmentRepository>();
        _mockClinicianRepo = new Mock<IClinicianRepository>();
        _mockPatientRepo = new Mock<IPatientRepository>();

        _appointmentService = new AppointmentService(
            _mockAppointmentRepo.Object,
            _mockPatientRepo.Object,
            _mockClinicianRepo.Object
        );
    }

    // GetAppointmentsBy and GetAppointmentWithDetails methods will be covered by integration testing at the repository layer
    // These are essentially pass through methods that contain no logic so are inappropriate for unit testing.


    // Add Appointment Unit Tests
    [Fact]
    public async Task AddAppointment_NullPatient_ReturnsFailureWithErrorMessage()
    {
        // Arrange
        _mockPatientRepo.Setup(mock => mock.GetPatientAsync(1)).ReturnsAsync((Patient?)null);
        
        var mockCreateAppointmentDTO = new CreateAppointmentDTO
        {
            PatientId = 1,
            Department = "TestDepartment",
            ClinicianId = 5000,
            AppointmentTime = new DateTime(2025, 2, 15, 12, 0, 0)
        };

        // Act
        var result = await _appointmentService.AddAppointmentAsync(mockCreateAppointmentDTO);

        // Assert
        Assert.False(result.WasSuccessful);
        Assert.Equal("Patient with this ID does not exist.", result.Message);
    }

    [Fact]
    public async Task AddAppointment_NullClinician_ReturnsFailureWithErrorMessage()
    {
        // Arrange
        Patient mockPatient = new Patient
        {
            Id = 1,
            FirstName = "Test",
            LastName = "Testson",
            Dob = new DateOnly(1990, 10, 21),
            NhsNumber = "0123456789",
            Address = "101 Test Drive"
        };

        _mockPatientRepo.Setup(mock => mock.GetPatientAsync(1)).ReturnsAsync(mockPatient);
        _mockClinicianRepo.Setup(mock => mock.GetClinicianAsync(1)).ReturnsAsync((Clinician?)null);
        var mockCreateAppointmentDTO = new CreateAppointmentDTO
        {
            PatientId = 1,
            Department = "TestDepartment",
            ClinicianId = 1,
            AppointmentTime = new DateTime(2024, 1, 12, 15, 0, 0)
        };

        // Act
        var result = await _appointmentService.AddAppointmentAsync(mockCreateAppointmentDTO);

        // Assert
        Assert.False(result.WasSuccessful);
        Assert.Equal("Clinician with this ID does not exist.", result.Message);
    }

    [Fact]
    public async Task AddAppointment_ValidPatientClinicianAndAppointmentDTO_ReturnsSuccessWithNewAppointment()
    {
        // Arrange
        Patient mockPatient = new Patient
        {
            Id = 3,
            FirstName = "Test",
            LastName = "Testson",
            Dob = new DateOnly(1990, 10, 21),
            NhsNumber = "0123456789",
            Address = "101 Test Drive"
        };

        Clinician mockClinician = new Clinician
        {
            Id = 4,
            FirstName = "Test",
            LastName = "Testson",
            Dob = new DateOnly(1990, 10, 21),
            GmcNumber = "01234567",
            Specialty = "Testing"
        };

        _mockPatientRepo.Setup(mock => mock.GetPatientAsync(3)).ReturnsAsync(mockPatient);
        _mockClinicianRepo.Setup(mock => mock.GetClinicianAsync(4)).ReturnsAsync(mockClinician);
        
        var mockCreateAppointmentDTO = new CreateAppointmentDTO
        {
            PatientId = 3,
            Department = "TestDepartment",
            ClinicianId = 4,
            AppointmentTime = new DateTime(2024, 1, 12, 15, 0, 0)
        };

        // Act
        var result = await _appointmentService.AddAppointmentAsync(mockCreateAppointmentDTO);

        // Assert
        _mockAppointmentRepo.Verify(mock => mock.AddAndSaveAppointmentAsync(It.Is<Appointment>(a => a.ClinicianId == 4 
                                                                                               && a.PatientId == 3
                                                                                               && a.Department == "TestDepartment"
                                                                                               && a.AppointmentTime ==  new DateTime(2024, 1, 12, 15, 0, 0)
                                                                                               && a.Status == AppointmentStatus.Pending)), Times.Once);
        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.NewAppointment);
    }

    // Change Appointment Status Unit Tests
    [Fact]
    public async Task ChangeAppointmentStatus_NullAppointment_ReturnsFailureWithMessage()
    {

        // Arrange
        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(1))
            .ReturnsAsync((Appointment?)null);

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 1,
            Status = AppointmentStatus.Completed
        };

        // Act
        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

        // Assert
         Assert.False(result.wasSuccessful);
         Assert.Equal("Appointment not found", result.message);
    }

        [Fact]
        public async Task ChangeAppointmentStatus_AlreadySameStatus_ReturnsSuccessWithAlreadyStatusMessage()
    {

        //Arrange
        Appointment mockAppointment = new Appointment
            {
                Id = 5,
                PatientId = 1,
                Department = "Colon",
                ClinicianId = 1,
                Status = AppointmentStatus.EnteredInError,
                AppointmentTime = new DateTime(2026, 5, 5, 14, 0, 0)
            } ;

        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(5))
            .ReturnsAsync(mockAppointment);

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 5,
            Status = AppointmentStatus.EnteredInError
        };

        // Act
        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

        // Assert
        _mockAppointmentRepo.Verify(mock => mock.UpdateStatus(It.IsAny<Appointment>(), It.IsAny<AppointmentStatus>()), Times.Never);
         Assert.True(result.wasSuccessful);
         Assert.Equal("Appointment was already EnteredInError.\nAppointment Status : EnteredInError", result.message);
    }

        [Fact]
        public async Task ChangeAppointmentStatus_SuccessfullyChanged_ReturnsSuccessWithSuccessMessage()
    {

        // Arrange
        Appointment mockAppointment = new Appointment
            {
                Id = 4,
                PatientId = 2,
                Department = "Test",
                ClinicianId = 3,
                Status = AppointmentStatus.Pending,
                AppointmentTime = new DateTime(2026, 10, 5, 20, 0, 0)
            } ;

        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(4))
            .ReturnsAsync(mockAppointment);

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 4,
            Status = AppointmentStatus.Completed
        };

        // Act
        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

        // Assert
        _mockAppointmentRepo.Verify(mock => mock.UpdateStatus(mockAppointment, mockChangeAppointmentDTO.Status), Times.Once);

         Assert.True(result.wasSuccessful); 
         Assert.Equal("Appointment status successfully changed to Completed", result.message);
    }
}
