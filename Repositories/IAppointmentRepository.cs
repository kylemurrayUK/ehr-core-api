namespace EHRCoreAPI
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAllAppointments();
        Appointment? GetAppointment(int id);
        Appointment? GetAppointmentWithDetails(int id);
        List<Appointment> GetPatientAppointments(int patientID);
        List<Appointment> GetClinicianAppointments(int clinicianID);
        List<Appointment> GetDepartmentAppointments(string department);
        void AddAndSaveAppointment(Appointment NewAppointment);
        void UpdateStatus (Appointment appointment, AppointmentStatus newAppointmentStatus);
        public List<Appointment> GetAppointmentBy(int? patientId = null, int? clinicianId = null, 
        string? department = null, string? patientName = null, string? clinicianName = null);

    }

 } 


