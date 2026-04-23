namespace EHRCoreAPI
{
    public class Clinician : Person
    {
        public string GmcNumber {get; set;} = string.Empty;
        public string Specialty {get; set;} = string.Empty;

        public Clinician (int Id, string firstName, string lastName, DateOnly dob, 
                        string gmcNumber, string specialty) : base (Id, firstName, lastName, dob)
        {
            GmcNumber = gmcNumber;
            Specialty = specialty;
        }
    }
}