namespace EHRCoreAPI
{
    public class ReturnAppointmentDTO
    {
        public int Id {get; set;}
        public required PatientSummaryDTO Patient {get; set;}
        public required ClinicianSummaryDTO Clinician {get; set;}
        public string Department {get; set;} = string.Empty;
        public AppointmentStatus Status {get; set;}
        public DateTime AppointmentTime {get; set;}

    }
}