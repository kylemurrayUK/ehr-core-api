namespace EHRCoreAPI
{
    public class CreateAppointmentStatus
    {
        public bool WasSuccessful {get;}
        public Appointment? NewAppointment {get;}
        public string? Message {get;}

        private CreateAppointmentStatus(bool wassuccessful, Appointment? appointment, string? message)
        {
            WasSuccessful = wassuccessful;
            NewAppointment = appointment;
            Message = message;
        }

        public static CreateAppointmentStatus Success(Appointment appointment)
        {
            return new CreateAppointmentStatus(true, appointment, null);
        }
        
        public static CreateAppointmentStatus Failure(string message)
        {
            return new CreateAppointmentStatus(false, null, message);
        }
    }
}