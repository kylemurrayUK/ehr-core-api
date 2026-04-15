using System.ComponentModel.DataAnnotations;

namespace EHRCoreAPI
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
        public int PatientId {get; set;}

        /// <summary>
        /// Department appointment is assigned to. Required field and is initalised empty.
        /// </summary>
        [Required]
        public string Department {get; set;} = string.Empty;

        /// <summary>
        /// Clinician assigned to appointments name. Required field and is initalised empty.
        /// </summary>
        [Required]
        public int ClinicianId {get; set;}

        /// <summary>
        /// Whether appointment is Pending, Completed, Cancelled or Entered in error
        /// </summary>
        public AppointmentStatus Status {get; set;}

        /// <summary>
        /// Time and date of appointment
        /// </summary>
        [Required]
        public DateTime AppointmentTime {get; set;}

        public Appointment(int id,int patientId, string department,
                           int clinicianId, AppointmentStatus status, DateTime appointmentTime)
        {
            Id = id;
            PatientId = patientId;
            Department = department;
            ClinicianId = clinicianId;
            Status = status;
            AppointmentTime = appointmentTime;
        }

    }
}