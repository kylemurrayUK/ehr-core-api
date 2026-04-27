namespace EHRCoreAPI
{
    public class AppointmentRepository : IAppointmentRespository
    {
        private readonly ApiDbContext _db;
        public List<Appointment> GetAllAppointments()
        {
            return _db.Appointments.ToList();
        }
        public Appointment? GetAppointment(int id)
        {
            return _db.Appointments.FirstOrDefault(a => a.Id == id);
        }
        public List<Appointment> GetPatientAppointments(int patientID)
        {
            return _db.Appointments.Where(a => a.PatientId == patientID).ToList();
        }
        public List<Appointment> GetClinicianAppointments(int clinicianID)
        {
            return _db.Appointments.Where(a => a.ClinicianId == clinicianID).ToList();
        }
        public List<Appointment> GetDepartmentAppointments(string department)
        {
            return _db.Appointments.Where(a => a.Department == department).ToList();
        }
        public void AddAndSaveAppointment(Appointment NewAppointment)
        {

        }


    }
}