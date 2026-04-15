using System.ComponentModel.DataAnnotations;

namespace EHRCoreAPI
{
    /// <summary>
    /// Represents data needed to create an appointment
    /// </summary>
    public class CreateAppointmentDTO
    {
        /// <summary>
        /// Patient name
        /// </summary>
        [Required(ErrorMessage = "Patient Id is required.")]
        public int PatientId {get; set;}
        /// <summary>
        /// Department appoint sits with
        /// </summary>
        [Required(ErrorMessage = "Department name is required.")]
        public string Department {get; set;} = string.Empty;
        /// <summary>
        /// Clinician patient will be seeing
        /// </summary>
        [Required(ErrorMessage = "Clinician Id name is required.")]
        public int ClinicianId {get; set;}
        [Required(ErrorMessage = "Date and time of appointment is required. ")]
        public DateTime AppointmentTime{get; set;}
    }
}