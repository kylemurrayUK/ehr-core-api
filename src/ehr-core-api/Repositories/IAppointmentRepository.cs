using EHRCoreAPI.Models;

namespace EHRCoreAPI.Repositories
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAppointmentsAsync(int? clinicianId = null);
        Task<Appointment?> GetAppointmentAsync(int id);
        Task<Appointment?> GetAppointmentWithDetailsAsync(int id);
        Task AddAndSaveAppointmentAsync(Appointment NewAppointment);
        Task UpdateStatus (Appointment appointment, AppointmentStatus? newAppointmentStatus);
        Task<List<Appointment>> GetAppointmentByAsync(FilterParameters filters, int? clinicianId = null);
    }

 } 


