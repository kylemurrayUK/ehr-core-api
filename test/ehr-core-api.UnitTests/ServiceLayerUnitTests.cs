using EHRCoreAPI.Repositories.Implementations;
using EHRCoreAPI.Repositories;
using EHRCoreAPI;
using Microsoft.EntityFrameworkCore.Query;
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
    public async Task ChangeAppointmentStatus_NullAppointment_ReturnsFalseTuppleWithMessage()
    {
        _mockAppointmentRepo
            .Setup(mock => mock.GetAppointmentAsync(It.IsAny<int>()))
            .ReturnsAsync((Appointment?)null);

        var mockChangeAppointmentDTO = new ChangeAppointmentStatusDTO
        {
            Id = 1,
            Status = AppointmentStatus.Completed
        };

        var result = await _appointmentService.ChangeAppointmentStatus(mockChangeAppointmentDTO);

         Assert.True(result.wasSuccessful == false, result.message = "Appointment not found");
    }
}
