namespace EHRCoreAPI.Models
{
    public class FilterParameters
    {
        
        public FilterParameters (int? patientId, int? clinicianId, string? department, 
                                string? patientName, string? clinicianName, AppointmentStatus? status)
        {
            PatientId = patientId;
            ClinicianId = clinicianId;
            Department = department;
            PatientName = patientName;
            ClinicianName = clinicianName;
            Status = status;
        }
        public int? PatientId {get; set;}
        public int? ClinicianId {get; set;}
        public string? Department {get; set;}
        public string? PatientName {get; set;}
        public string? ClinicianName {get; set;}
        public AppointmentStatus? Status {get; set;}
    }
}