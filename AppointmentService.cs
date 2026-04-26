
namespace EHRCoreAPI
{
    public class  AppointmentService
    {

        private readonly ApiDbContext _db;
        public AppointmentService(ApiDbContext db)
        {
            _db = db;

        }

        public List<Appointment> ListAppointments()
        {
            return _db.Appointments.ToList();
        }
        
        public Appointment? GetAppointment(int iD)
        {
            return _db.Appointments.FirstOrDefault(a => a.Id == iD);
        }


        public List<Appointment> GetPatientAppointments(int patientID)
        {
            return _db.Appointments.Where(a => a.PatientId == patientID).ToList();
        }
        public List<Appointment> GetClinicianAppointments(int clinicianID)
        {
            return _db.Appointments.Where(a => a.ClinicianId == clinicianID).ToList();
        }
        public List<Appointment> GetDepartmentAppointments(string department)
        {
            return _db.Appointments.Where(a => a.Department == department).ToList();
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
            bool wasSuccessful = false;
            string message = "Appointment not found";

            if (_db.Appointments.Any(a => a.Id == changeAppointmentStatusDTO.Id))
            {
                foreach(Appointment appointment in _db.Appointments)
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
                _db.SaveChanges();
            }
            return (wasSuccessful, message);
        }
        


    }
}