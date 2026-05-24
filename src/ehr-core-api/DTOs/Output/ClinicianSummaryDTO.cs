namespace EHRCoreAPI.Dtos
{
    public class ClinicianSummaryDTO
    {
        public int Id {get; set;}
        public string Name {get; init;} = string.Empty;
        public string Specialty  {get; init;} = string.Empty;
    }
}