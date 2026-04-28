
namespace EHRCoreAPI
{
    public class  AppointmentService
    {

        private readonly IAppointmentRespository _appointmentRespository;
        public AppointmentService(IAppointmentRespository appointmentRespository)
        {
            _appointmentRespository = appointmentRespository;
        }

        public List<Appointment> ListAppointments()
        {
            return _appointmentRespository.GetAllAppointments();
        }
        
        public Appointment? GetAppointment(int id)
        {
            return _appointmentRespository.GetAppointment(id);
        }


        public List<Appointment> GetPatientAppointments(int patientID)
        {
            return _appointmentRespository.GetPatientAppointments(patientID);
        }
        public List<Appointment> GetClinicianAppointments(int clinicianID)
        {
            return _appointmentRespository.GetClinicianAppointments(clinicianID);
        }
        public List<Appointment> GetDepartmentAppointments(string department)
        {
            return _appointmentRespository.GetDepartmentAppointments(department);
        }

        public CreateAppointmentStatus AddAppointment(CreateAppointmentDTO createAppointmentDTO)
        {
            if (!_db.Patients.Any(p => p.Id == createAppointmentDTO.PatientId))
            {
                return CreateAppointmentStatus.Failure("Patient with this ID does not exist.");
            }
            if (!_db.Clinicians.Any(c => c.Id == createAppointmentDTO.ClinicianId))
            {
                return CreateAppointmentStatus.Failure("Clinician with this ID does not exist.");
            }

            Appointment newAppointment = new Appointment{ PatientId = createAppointmentDTO.PatientId!.Value, Department = createAppointmentDTO.Department, ClinicianId = createAppointmentDTO.ClinicianId!.Value,
                                                          Status = AppointmentStatus.Pending, AppointmentTime = createAppointmentDTO.AppointmentTime};
            _db.Appointments.Add(newAppointment);
            _db.SaveChanges();
            return CreateAppointmentStatus.Success(newAppointment);
        }

        // No delete method as in a medical context you would want to keep all appointments
        // - even cancelled ones - for auditing purposes.
        public (bool wasSuccessful, string message) ChangeAppointmentStatus(ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == changeAppointmentStatusDTO.Id);

            if(appointment == null)
            {
                return (false, "Appointment not found");
            }
            
            if (appointment.Status == changeAppointmentStatusDTO.Status)
            {
                return ( true, $"Appointment was already {changeAppointmentStatusDTO.Status}.\n" + 
                $"Appointment Status : {appointment.Status}");

            } 

            appointment.Status = changeAppointmentStatusDTO.Status;
            _db.SaveChanges();
            return (true, $"Appointment status successfully changed to {changeAppointmentStatusDTO.Status}");
        }
    }
}