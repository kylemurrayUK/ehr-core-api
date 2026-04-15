namespace AppointmentManagementAPI
{
    public class  AppointmentService
    {
        private List<Appointment> _appointments;
        private IFileStorage _fileStorage;

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


        public List<Appointment> GetPatientAppointments(string patient)
        {
            return _appointments.Where(a => a.Patient == patient).ToList();
        }
        public List<Appointment> GetClinicianAppointments(string clinician)
        {
            return _appointments.Where(a => a.Clinician == clinician).ToList();
        }
        public List<Appointment> GetDepartmentAppointments(string department)
        {
            return _appointments.Where(a => a.Department == department).ToList();
        }

        public Appointment AddAppointment(CreateAppointmentDTO createAppointmentDTO)
        {
            int id = FindNextID(_appointments);
            Appointment newAppointment = new Appointment(id, createAppointmentDTO.Patient, createAppointmentDTO.Department, createAppointmentDTO.Clinician, AppointmentStatus.Pending, createAppointmentDTO.AppointmentTime);
            _appointments.Add(newAppointment);
            _fileStorage.SaveFile(_appointments);
            return newAppointment;
        }

        // No delete method as in a medical conext you would want to keep all appointments
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