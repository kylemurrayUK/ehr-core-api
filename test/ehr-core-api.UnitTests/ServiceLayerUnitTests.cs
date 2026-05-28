using EHRCoreAPI.Repositories;
using EHRCoreAPI;
using EHRCoreAPI.Models;
using EHRCoreAPI.Dtos;



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

    [Fact]
    public async Task ChangeAppointmentStatus_NullAppointment_ReturnsFailureWithMessage()
    {
        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(1))
            .ReturnsAsync((Appointment?)null);

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 1,
            Status = AppointmentStatus.Completed
        };

        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

         Assert.True(!result.wasSuccessful);
         Assert.Equal("Appointment not found", result.message);
    }

        [Fact]
        public async Task ChangeAppointmentStatus_AlreadySameStatus_ReturnsSuccessWithAlreadyStatusMessage()
    {
        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(5))
            .ReturnsAsync(new Appointment
            {
                Id = 5,
                PatientId = 1,
                Department = "Colon",
                ClinicianId = 1,
                Status = AppointmentStatus.EnteredInError,
                AppointmentTime = new DateTime(2026, 5, 5, 14, 0, 0)
            });

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 5,
            Status = AppointmentStatus.EnteredInError
        };

        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

         Assert.True(result.wasSuccessful);
         Assert.Equal("Appointment was already EnteredInError.\nAppointment Status : EnteredInError", result.message);
    }

        [Fact]
        public async Task ChangeAppointmentStatus_SuccessfullyChanged_ReturnsSuccessWithSuccessMessage()
    {
        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(4))
            .ReturnsAsync(new Appointment
            {
                Id = 4,
                PatientId = 2,
                Department = "Test",
                ClinicianId = 3,
                Status = AppointmentStatus.EnteredInError,
                AppointmentTime = new DateTime(2026, 10, 5, 20, 0, 0)
            });

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 4,
            Status = AppointmentStatus.Completed
        };

        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

         Assert.True(result.wasSuccessful); 
         Assert.Equal("Appointment status successfully changed to Completed", result.message);
    }
}
