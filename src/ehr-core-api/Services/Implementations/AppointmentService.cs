using EHRCoreAPI.Dtos;
using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories;


namespace EHRCoreAPI.Services.Implementations
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

        public async Task<List<Appointment>> ListAppointmentsAsync(int? clinicianId = null)
        {

            return await _appointmentRepository.GetAllAppointmentsAsync(clinicianId);
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int id)
        {
            return await _appointmentRepository.GetAppointmentWithDetailsAsync(id);
        }

        public async Task<List<Appointment>> GetAppointmentByAsync(FilterParameters filters, int? clinicianId = null)
        {

            return await _appointmentRepository.GetAppointmentByAsync(filters, clinicianId);
        }


        public async Task<CreateAppointmentStatus> AddAppointmentAsync(CreateAppointmentDTO createAppointmentDTO)
        {
            int patientId = createAppointmentDTO.PatientId ?? throw new ArgumentException("PatientId is null");
            int clinicianId = createAppointmentDTO.ClinicianId ?? throw new ArgumentException("Clinician is null");
            

            var patient = await _patientRepository.GetPatientAsync(patientId);
            var clinician = await _clinicianRepository.GetClinicianAsync(clinicianId);
            if (patient == null)
            {
                return CreateAppointmentStatus.Failure("Patient with this ID does not exist.");
            }
            if (clinician == null)
            {
                return CreateAppointmentStatus.Failure("Clinician with this ID does not exist.");
            }
            
            Appointment newAppointment = new Appointment{ PatientId = patientId, 
                                                          Department = createAppointmentDTO.Department, 
                                                          ClinicianId = clinicianId,
                                                          Status = AppointmentStatus.Pending,
                                                          AppointmentTime = createAppointmentDTO.AppointmentTime ?? throw new NullReferenceException("Appointment time is null")};
            await _appointmentRepository.AddAndSaveAppointmentAsync(newAppointment);

            return CreateAppointmentStatus.Success(newAppointment);
        }

        // No delete method as in a medical context you would want to keep all appointments
        // - even cancelled ones - for auditing purposes.
        public async Task<(bool wasSuccessful, string message)> ChangeAppointmentStatus(ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            // ! used here as changeAppointmentStatusDTO's id has been validated as not null by the controller
            var appointment = await _appointmentRepository.GetAppointmentAsync(changeAppointmentStatusDTO.Id!.Value);

            if(appointment == null)
            {
                return (false, "Appointment not found");
            }
            
            if (appointment.Status == changeAppointmentStatusDTO.Status)
            {
                return ( true, $"Appointment was already {changeAppointmentStatusDTO.Status}.\n" + 
                $"Appointment Status : {appointment.Status}");

            } 

            await _appointmentRepository.UpdateStatus(appointment, changeAppointmentStatusDTO.Status);
            return (true, $"Appointment status successfully changed to {changeAppointmentStatusDTO.Status}");
        }
    }
}