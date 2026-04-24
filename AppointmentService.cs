
namespace EHRCoreAPI
{
    public class  AppointmentService
    {
        private List<Appointment> _appointments;
        private IFileStorage _fileStorage;

        List<Patient> _patients = new List<Patient>()
        {
            new Patient(1, "Kyle", "Murray", new DateOnly(1997, 6, 16), "1234567", "16 Lidgate Close"),
            new Patient(2, "Declan", "Mahoney", new DateOnly(1996, 9, 16), "7654321", "1 New York Street"),
            new Patient(3, "Max", "Briggds", new DateOnly(1999, 5, 20), "4536271", "Somewhere in Leeds"),
            new Patient(4, "Ali", "Grundy", new DateOnly(1997, 6, 15), "6253417", "Warwick Way")
        };

        List<Clinician> _clinicians = new List<Clinician>()
        {
            new Clinician(1, "Johhny", "Doc", new DateOnly(1950, 5, 1), "0987654", "Surgery"),
            new Clinician(2, "William", "Murray", new DateOnly(1959, 12, 17), "4567890", "Cardiology"),
            new Clinician(3, "Susan", "Murray", new DateOnly(1958, 11, 9), "7869504", "Gastrointestinal"),
            new Clinician(4, "Margaret", "Neil", new DateOnly(1958, 10, 2), "4059687", "Pharmacology")
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