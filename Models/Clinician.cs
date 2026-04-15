namespace EHRCoreAPI
{
    public class Clinician : Person
    {
        public string GmcNumber {get; set;} = string.Empty;
        public string Speciality {get; set;} = string.Empty;
    }
}