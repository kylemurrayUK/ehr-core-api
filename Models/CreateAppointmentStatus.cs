namespace EHRCoreAPI
{
    public class CreateAppointmentStatus
    {
        public bool WasSuccessful {get;}
        public ReturnAppointmentDTO? NewAppointment {get;}
        public string? Message {get;}

        private CreateAppointmentStatus(bool wasSuccessful, ReturnAppointmentDTO? appointment, string? message)
        {
            WasSuccessful = wasSuccessful;
            NewAppointment = appointment;
            Message = message;
        }

        public static CreateAppointmentStatus Success(ReturnAppointmentDTO appointment)
        {
            return new CreateAppointmentStatus(true, appointment, null);
        }
        
        public static CreateAppointmentStatus Failure(string message)
        {
            return new CreateAppointmentStatus(false, null, message);
        }
    }
}