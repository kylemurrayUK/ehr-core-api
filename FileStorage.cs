using System.Text.Json;

namespace AppointmentManagementAPI
{
    /// <summary>
    /// Class responsible for handling file loading/saving for appointments.
    /// </summary>

    public class FileStorage : IFileStorage
    {
        static readonly string filePath = Path.Combine("data", "Appointments.json");
        /// <summary>
        /// Loads Appointments.json file and converts into a List
        /// </summary>
        /// <returns>A list of type appointments from memory. 
        /// Empty list of type appointments returned if exceptions are thrown</returns>
        public List<Appointment> LoadFile()
        {
            List<Appointment>? loadedAppointments;
            if (!Directory.Exists(@"data"))
            {
                Directory.CreateDirectory(@"data");
            }
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }
            try
            {
                loadedAppointments = JsonSerializer.Deserialize<List<Appointment>>(File.ReadAllText(filePath));
            }
            catch (JsonException)
            {
                Console.WriteLine("Json exception thrown - invalid JSON in file. Empty list returned");
                return new List<Appointment>();
            }
            if (loadedAppointments == null)
            {
                Console.WriteLine("null file. Empty list returned");
                return new List<Appointment>();
            }
            return loadedAppointments;

        }
        /// <summary>
        /// Overrites current list in appointments.json with appointments list inserted in parameters
        /// </summary>
        /// <param name="appointments">List of type appointments</param>
        public void SaveFile(List<Appointment> appointments)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize<List<Appointment>>(appointments));   
        }
    }
}