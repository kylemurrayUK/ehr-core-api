namespace EHRCoreAPI
{
    public class ReturnAppointmentDTO
    {
        public int Id {get; set;}
        public PatientSummaryDTO PatientSummary {get; set;}
        public ClinicianSummaryDTO ClinicianSummary {get; set;}
        public string Department {get; set;} = string.Empty;
        public AppointmentStatus Status {get; set;}
        public DateTime AppointmentTime {get; set;}

        public ReturnAppointmentDTO(int id, PatientSummaryDTO patientSummary, 
                                    ClinicianSummaryDTO clinicianSummary, 
                                    string department, AppointmentStatus status,
                                    DateTime appointmentTime)
        {
            Id = id;
            PatientSummary = patientSummary;
            ClinicianSummary = clinicianSummary;
            Department = department;
            Status = status;
            AppointmentTime = appointmentTime;
        }
    }
}