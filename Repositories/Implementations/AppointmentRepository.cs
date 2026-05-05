using Microsoft.EntityFrameworkCore;

namespace EHRCoreAPI
{
    public class AppointmentRepository : IAppointmentRespository
    {
        private readonly ApiDbContext _db;
        public AppointmentRepository(ApiDbContext db)
        {
            _db = db;
        }
        public List<Appointment> GetAllAppointments()
        {
            return _db.Appointments.ToList();
        }
        public Appointment? GetAppointment(int id)
        {
            return _db.Appointments.FirstOrDefault(a => a.Id == id);
        }
        public Appointment? GetAppointmentWithDetails(int id)
        {
            return _db.Appointments.Include(a => a.Patient).Include(a => a.Clinician).FirstOrDefault(a => a.Id == id);
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
        public void AddAndSaveAppointment(Appointment newAppointment)
        {
            _db.Appointments.Add(newAppointment);
            _db.SaveChanges();
        }
        public void UpdateStatus (Appointment appointment, AppointmentStatus newAppointmentStatus)
        {
            appointment.Status = newAppointmentStatus;
            _db.SaveChanges();
        }
        public List<Appointment> GetAppointmentBy(int? patientId = null, int? clinicianId = null, 
                                                  string? department = null, string? patientName = null, string? clinicianName = null)
    {
        IQueryable<Appointment> query =  _db.Appointments;   
        if (patientId != null)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        } 
        if (clinicianId != null)
        {
            query = query.Where(a => a.ClinicianId == clinicianId.Value);
        } 
        if (department != null)
        {
            query = query.Where(a => a.Department == department);
        }
        if (patientName != null)
        {
            query = query.Include(a => a.Patient).Where(a => a.Patient.FirstName.Contains(patientName) || a.Patient.LastName.Contains(patientName));
        } 
        if (clinicianName != null)
        {
            query = query.Include(a => a.Clinician).Where(a => a.Clinician.FirstName.Contains(clinicianName) || a.Clinician.LastName.Contains(clinicianName));
        } 
        return query.ToList();
    }

    }
}