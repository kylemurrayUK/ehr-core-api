namespace EHRCoreAPI
{
    public class ClinicianSummaryDTO
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Speciality {get; set;}

        public ClinicianSummaryDTO(int id, string name, string speciality)
        {
            Id = id;
            Name = name;
            Speciality = speciality;
        }
    }
}