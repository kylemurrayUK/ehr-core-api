namespace EHRCoreAPI
{
    public class PatientSummaryDTO
    {
        public int Id {get; set;} 
        public string Name {get; set;}
        public string Speciality {get; set;}

        public PatientSummaryDTO(int id,string name, string speciality)
        {
            Id = id;
            Name = name;
            Speciality = speciality;
        }
    }
}