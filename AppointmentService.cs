
namespace EHRCoreAPI
{
    public class  AppointmentService
    {
        private List<Appointment> _appointments;
        private IFileStorage _fileStorage;

        List<Patient> _patients = new List<Patient>()
        {

        };

        List<Clinician> _clinicians = new List<Clinician>()
        {

        };



        public AppointmentService(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
            _appointments = fileStorage.LoadFile();
        }

        public List<Appointment> ListAppointments()
        {
            return _appointments;
        }
        
        public Appointment? GetAppointment(int iD)
        {
            return _appointments.FirstOrDefault(a => a.Id == iD);
        }


        public List<Appointment> GetPatientAppointments(int patientID)
        {
            return _appointments.Where(a => a.PatientId == patientID).ToList();
        }
        public List<Appointment> GetClinicianAppointments(int clinicianID)
        {
            return _appointments.Where(a => a.ClinicianId == clinicianID).ToList();
        }
        public List<Appointment> GetDepartmentAppointments(string department)
        {
            return _appointments.Where(a => a.Department == department).ToList();
        }

        public CreateAppointmentStatus AddAppointment(CreateAppointmentDTO createAppointmentDTO)
        {
            if (!_patients.Any(p => p.Id == createAppointmentDTO.PatientId))
            {
                return CreateAppointmentStatus.Failure("Patient with this ID does not exist.");
            }
            if (!_clinicians.Any(c => c.Id == createAppointmentDTO.ClinicianId))
            {
                return CreateAppointmentStatus.Failure("Clinician with this ID does not exist.");
            }

            int id = FindNextID(_appointments);
            Appointment newAppointment = new Appointment(id, createAppointmentDTO.PatientId, createAppointmentDTO.Department, createAppointmentDTO.ClinicianId, AppointmentStatus.Pending, createAppointmentDTO.AppointmentTime);
            _appointments.Add(newAppointment);
            _fileStorage.SaveFile(_appointments);
            return CreateAppointmentStatus.Success(newAppointment);
        }

        // No delete method as in a medical context you would want to keep all appointments
        // - even cancelled ones - for auditing purposes.
        public (bool wasSuccessful, string message) ChangeAppointmentStatus(ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            bool wasSuccessful = false;
            string message = "Appointment not found";

            if (_appointments.Any(a => a.Id == changeAppointmentStatusDTO.Id))
            {
                foreach(Appointment appointment in _appointments)
                {
                    if(appointment.Id == changeAppointmentStatusDTO.Id)
                    {
                        if (appointment.Status == changeAppointmentStatusDTO.Status)
                        {
                            message = $"Appointment was already {changeAppointmentStatusDTO.Status}.\n" + 
                                        $"Appointment Status : {appointment.Status}";
                            wasSuccessful = true;
                        } 
                        else
                        {
                            appointment.Status = changeAppointmentStatusDTO.Status;
                            message = $"Appointment status successfully changed to {changeAppointmentStatusDTO.Status}";
                            wasSuccessful = true;
                        }

                    }
                }
                _fileStorage.SaveFile(_appointments);
            }
            return (wasSuccessful, message);
        }
        

        private int FindNextID(List<Appointment> appointments)
        {
            if (appointments.Count() == 0)
            {
                return 1;
            }
            return appointments.Max(a => a.Id) + 1;
        }
    }
}