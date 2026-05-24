using EHRCoreAPI.Models;
using EHRCoreAPI.Dtos.Output;

namespace EHRCoreAPI.Mappers
{
    public static class AppointmentOutputDTOMapper
    {
        public static ReturnAppointmentDTO ToReturnDTO(this Appointment appointment, 
        PatientSummaryDTO patientSummary, ClinicianSummaryDTO clinicianSummary)
        {
            ReturnAppointmentDTO returnAppointment = new ReturnAppointmentDTO
            {
                Id = appointment.Id,
                Patient = patientSummary,
                Clinician = clinicianSummary,
                Department = appointment.Department,
                Status = appointment.Status,
                AppointmentTime = appointment.AppointmentTime
            };

            return returnAppointment;
        }
    }
}