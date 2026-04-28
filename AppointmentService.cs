
namespace EHRCoreAPI
{
    public class  AppointmentService
    {

        private readonly IAppointmentRespository _appointmentRespository;
        private readonly IPatientRepository _patientRespository;
        private readonly IClinicianRepository _clinicianRespository;

        public AppointmentService(IAppointmentRespository appointmentRespository, IPatientRepository patientRespository, 
                                  IClinicianRepository clinicianRespository)
        {
            _appointmentRespository = appointmentRespository;
            _patientRespository = patientRespository;
            _clinicianRespository = clinicianRespository;
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
            if (_patientRespository.GetPatient(createAppointmentDTO.PatientId!.Value) == null)
            {
                return CreateAppointmentStatus.Failure("Patient with this ID does not exist.");
            }
            if (_clinicianRespository.GetClinician(createAppointmentDTO.ClinicianId!.Value) == null)
            {
                return CreateAppointmentStatus.Failure("Clinician with this ID does not exist.");
            }

            Appointment newAppointment = new Appointment{ PatientId = createAppointmentDTO.PatientId!.Value, Department = createAppointmentDTO.Department, ClinicianId = createAppointmentDTO.ClinicianId!.Value,
                                                          Status = AppointmentStatus.Pending, AppointmentTime = createAppointmentDTO.AppointmentTime};
            _appointmentRespository.AddAndSaveAppointment(newAppointment);
            return CreateAppointmentStatus.Success(newAppointment);
        }

        // No delete method as in a medical context you would want to keep all appointments
        // - even cancelled ones - for auditing purposes.
        public (bool wasSuccessful, string message) ChangeAppointmentStatus(ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            var appointment = _appointmentRespository.GetAppointment(changeAppointmentStatusDTO.Id!.Value);

            if(appointment == null)
            {
                return (false, "Appointment not found");
            }
            
            if (appointment.Status == changeAppointmentStatusDTO.Status)
            {
                return ( true, $"Appointment was already {changeAppointmentStatusDTO.Status}.\n" + 
                $"Appointment Status : {appointment.Status}");

            } 

            _appointmentRespository.UpdateStatus(appointment, changeAppointmentStatusDTO.Status);
            return (true, $"Appointment status successfully changed to {changeAppointmentStatusDTO.Status}");
        }
    }
}