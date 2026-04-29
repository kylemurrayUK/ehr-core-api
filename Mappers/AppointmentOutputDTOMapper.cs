namespace EHRCoreAPI
{
    public static class AppointmentOutputDTOMapper
    {
        public static ReturnAppointmentDTO ToReturnDTO(this Appointment appointment, 
        PatientSummaryDTO patientSummary, ClinicianSummaryDTO clinicianSummary)
        {
            ReturnAppointmentDTO returnAppointment = new ReturnAppointmentDTO
            (
                appointment.Id,
                patientSummary,
                clinicianSummary,
                appointment.Department,
                appointment.Status,
                appointment.AppointmentTime
            );

            return returnAppointment;
        }
    }
}