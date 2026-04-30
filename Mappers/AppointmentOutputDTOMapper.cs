namespace EHRCoreAPI
{
    public static class AppointmentOutputDTOMapper
    {
        public static ReturnAppointmentDTO ToReturnDTO(this Appointment appointment, 
        PatientSummaryDTO patientSummary, ClinicianSummaryDTO clinicianSummary)
        {
            ReturnAppointmentDTO returnAppointment = new ReturnAppointmentDTO
            {
                Id =appointment.Id,
                PatientSummary = patientSummary,
                ClinicianSummary = clinicianSummary,
                Department = appointment.Department,
                Status = appointment.Status,
                AppointmentTime = appointment.AppointmentTime
            };

            return returnAppointment;
        }
    }
}