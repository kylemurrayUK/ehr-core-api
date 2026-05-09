namespace EHRCoreAPI
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAllAppointments();
        Appointment? GetAppointment(int id);
        Appointment? GetAppointmentWithDetails(int id);
        void AddAndSaveAppointment(Appointment NewAppointment);
        void UpdateStatus (Appointment appointment, AppointmentStatus newAppointmentStatus);
        public List<Appointment> GetAppointmentBy(FilterParameters filters);

    }

 } 


