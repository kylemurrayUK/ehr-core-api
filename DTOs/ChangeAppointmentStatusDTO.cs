using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementAPI
{
    /// <summary>
    /// Object that contains information needed to alter an appointments status
    /// </summary>
    public class ChangeAppointmentStatusDTO
    {
        [Required(ErrorMessage = "Appointment ID required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "New status is required")]
        public AppointmentStatus Status { get; set;}
    }
}