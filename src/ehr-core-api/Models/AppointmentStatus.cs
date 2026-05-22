using System.Text.Json.Serialization;

namespace EHRCoreAPI
{
    /// <summary>
    /// Enum for Appointment status
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppointmentStatus
    {
        Pending,
        Completed,
        Cancelled,
        EnteredInError
    }    
}
