using Microsoft.EntityFrameworkCore;

namespace EHRCoreAPI
{
    public class AppointmentRepository : IAppointmentRepository
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
        public async Task AddAndSaveAppointmentAsync(Appointment newAppointment)
        {
            _db.Appointments.Add(newAppointment);
            await _db.SaveChangesAsync();
        }
        public void UpdateStatus (Appointment appointment, AppointmentStatus newAppointmentStatus)
        {
            appointment.Status = newAppointmentStatus;
            _db.SaveChanges();
        }
        public List<Appointment> GetAppointmentBy(FilterParameters filters)
    {
        IQueryable<Appointment> query =  _db.Appointments.Include(a => a.Patient).Include(a => a.Clinician);   
        if (filters.PatientId != null)
        {
            query = query.Where(a => a.PatientId == filters.PatientId.Value);
        } 
        if (filters.ClinicianId != null)
        {
            query = query.Where(a => a.ClinicianId == filters.ClinicianId.Value);
        } 
        if (filters.Department != null)
        {
            query = query.Where(a => a.Department == filters.Department);
        }
        if (filters.PatientName != null)
        {
            query = query.Where(a => a.Patient.FirstName.Contains(filters.PatientName) || a.Patient.LastName.Contains(filters.PatientName));
        } 
        if (filters.ClinicianName != null)
        {
            query = query.Where(a => a.Clinician.FirstName.Contains(filters.ClinicianName) || a.Clinician.LastName.Contains(filters.ClinicianName));
        } 
        if (filters.Status != null)
        {
            query = query.Where(a => a.Status == filters.Status);    
        }
        return query.ToList();
    }

    }
}