using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementAPI
{
    /// <summary>
    /// Represents data needed to create an appointment
    /// </summary>
    public class CreateAppointmentDTO
    {
        /// <summary>
        /// Patient name
        /// </summary>
        [Required(ErrorMessage = "Patient Name is required.")]
        public string Patient {get; set;} = string.Empty;
        /// <summary>
        /// Department appoint sits with
        /// </summary>
        [Required(ErrorMessage = "Department name is required.")]
        public string Department {get; set;} = string.Empty;
        /// <summary>
        /// Clinician patient will be seeing
        /// </summary>
        [Required(ErrorMessage = "Clinician name is required.")]
        public string Clinician {get; set;} = string.Empty;
        [Required(ErrorMessage = "Date and time of appointment is required. ")]
        public DateTime AppointmentTime{get; set;}
    }
}