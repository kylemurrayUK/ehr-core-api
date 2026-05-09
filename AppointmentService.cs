
namespace EHRCoreAPI
{
    public class  AppointmentService
    {

        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicianRepository _clinicianRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, 
                                  IClinicianRepository clinicianRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _clinicianRepository = clinicianRepository;
        }

        public List<Appointment> ListAppointments()
        {
            return _appointmentRepository.GetAllAppointments();
        }

        public Appointment? GetAppointmentWithDetails(int id)
        {
            return _appointmentRepository.GetAppointmentWithDetails(id);
        }

        public List<Appointment> GetAppointmentBy(FilterParameters filters)
        {
            return _appointmentRepository.GetAppointmentBy(filters);
        }


        public CreateAppointmentStatus AddAppointment(CreateAppointmentDTO createAppointmentDTO)
        {
            // ! used because both patient and clinician ID has been verified as not being null by the controller
            var patient = _patientRepository.GetPatient(createAppointmentDTO.PatientId!.Value);
            var clinician = _clinicianRepository.GetClinician(createAppointmentDTO.ClinicianId!.Value);
            if (patient == null)
            {
                return CreateAppointmentStatus.Failure("Patient with this ID does not exist.");
            }
            if (clinician == null)
            {
                return CreateAppointmentStatus.Failure("Clinician with this ID does not exist.");
            }
            // ! used here as PatientId and clinicianID  have been validated as not null.
            Appointment newAppointment = new Appointment{ PatientId = createAppointmentDTO.PatientId!.Value, Department = createAppointmentDTO.Department, ClinicianId = createAppointmentDTO.ClinicianId!.Value,
                                                          Status = AppointmentStatus.Pending, AppointmentTime = createAppointmentDTO.AppointmentTime};
            _appointmentRepository.AddAndSaveAppointment(newAppointment);
            ReturnAppointmentDTO returnAppointment = newAppointment.ToReturnDTO(patient.ToPatientSummary(), clinician.ToClinicianSummary()); 
            return CreateAppointmentStatus.Success(returnAppointment);
        }

        // No delete method as in a medical context you would want to keep all appointments
        // - even cancelled ones - for auditing purposes.
        public (bool wasSuccessful, string message) ChangeAppointmentStatus(ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            // ! used here as changeAppointmentStatusDTO's id has been validated as not null by the controller
            var appointment = _appointmentRepository.GetAppointment(changeAppointmentStatusDTO.Id!.Value);

            if(appointment == null)
            {
                return (false, "Appointment not found");
            }
            
            if (appointment.Status == changeAppointmentStatusDTO.Status)
            {
                return ( true, $"Appointment was already {changeAppointmentStatusDTO.Status}.\n" + 
                $"Appointment Status : {appointment.Status}");

            } 

            _appointmentRepository.UpdateStatus(appointment, changeAppointmentStatusDTO.Status);
            return (true, $"Appointment status successfully changed to {changeAppointmentStatusDTO.Status}");
        }
    }
}