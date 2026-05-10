namespace EHRCoreAPI
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentAsync(int id);
        Task<Appointment?> GetAppointmentWithDetailsAsync(int id);
        Task AddAndSaveAppointmentAsync(Appointment NewAppointment);
        void UpdateStatus (Appointment appointment, AppointmentStatus newAppointmentStatus);
        public List<Appointment> GetAppointmentBy(FilterParameters filters);
    }

 } 


