using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementAPI
{
    /// <summary>
    /// Object that represents appointments and contains all information needed to manage them.
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Appointment unique ID. Automatically set with no user input.
        /// </summary>
        public int Id {get; set;}
        /// <summary>
        /// Patients name. Required field and is initalised empty.
        /// </summary>
        [Required]
        public string Patient {get; set;} = string.Empty;

        /// <summary>
        /// Department appointment is assigned to. Required field and is initalised empty.
        /// </summary>
        [Required]
        public string Department {get; set;} = string.Empty;

        /// <summary>
        /// Clinician assigned to appointments name. Required field and is initalised empty.
        /// </summary>
        [Required]
        public string Clinician {get; set;} = string.Empty;

        /// <summary>
        /// Whether appointment is Pending, Completed, Cancelled or Entered in error
        /// </summary>
        public AppointmentStatus Status {get; set;}

        /// <summary>
        /// Time and date of appointment
        /// </summary>
        [Required]
        public DateTime AppointmentTime {get; set;}

        public Appointment(int id,string patient, string department,
                           string clinician, AppointmentStatus status, DateTime appointmentTime)
        {
            Id = id;
            Patient = patient;
            Department = department;
            Clinician = clinician;
            Status = status;
            AppointmentTime = appointmentTime;
        }

    }
}