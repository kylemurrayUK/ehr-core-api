using EHRCoreAPI.Dtos;
using EHRCoreAPI.Mappers;
using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories;
using EHRCoreAPI.Dtos.Output;

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

        public async Task<List<ReturnAppointmentDTO>> ListAppointmentsAsync()
        {
            List<Appointment> allAppointments = await _appointmentRepository.GetAllAppointmentsAsync();
            List<ReturnAppointmentDTO> allAppointmentsAsDTOs = new List<ReturnAppointmentDTO>();

            foreach (Appointment appointment in allAppointments)
            {
                allAppointmentsAsDTOs.Add(appointment.ToReturnDTO(appointment.Patient.ToPatientSummary(), appointment.Clinician.ToClinicianSummary()));
            }
            return allAppointmentsAsDTOs;
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int id)
        {
            return await _appointmentRepository.GetAppointmentWithDetailsAsync(id);
        }

        public async Task<List<Appointment>> GetAppointmentByAsync(FilterParameters filters)
        {
            return await _appointmentRepository.GetAppointmentByAsync(filters);
        }


        public async Task<CreateAppointmentStatus> AddAppointmentAsync(CreateAppointmentDTO createAppointmentDTO)
        {
            // ! used because both patient and clinician ID has been verified as not being null by the controller
            var patient = await _patientRepository.GetPatientAsync(createAppointmentDTO.PatientId!.Value);
            var clinician = await _clinicianRepository.GetClinicianAsync(createAppointmentDTO.ClinicianId!.Value);
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