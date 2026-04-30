namespace EHRCoreAPI
{
    public class ClinicianSummaryDTO
    {
        public int Id {get; set;}
        public string Name {get; init;} = string.Empty;
        public string Speciality {get; init;} = string.Empty;
    }
}