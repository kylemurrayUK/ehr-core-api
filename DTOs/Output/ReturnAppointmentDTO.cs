namespace EHRCoreAPI
{
    public class ReturnAppointmentDTO
    {
        public int Id {get; set;}
        public required PatientSummaryDTO PatientSummary {get; set;}
        public required ClinicianSummaryDTO ClinicianSummary {get; set;}
        public string Department {get; set;} = string.Empty;
        public AppointmentStatus Status {get; set;}
        public DateTime AppointmentTime {get; set;}

    }
}