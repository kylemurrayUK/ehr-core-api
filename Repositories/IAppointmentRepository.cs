namespace EHRCoreAPI
{
    public interface IAppointmentRespository
    {
        List<Appointment> GetAllAppointments();
        Appointment GetAppointment(int id);
        List<Appointment> GetPatientAppointments(int patientID);
        List<Appointment> GetClinicianAppointments(int clinicianID);
        List<Appointment> GetDepartmentAppointments(string department);
        void AddAndSaveAppointment(Appointment NewAppointment);

    }
}