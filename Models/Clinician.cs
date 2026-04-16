namespace EHRCoreAPI
{
    public class Clinician : Person
    {
        public string GMCNumber {get; set;} = string.Empty;
        public string Speciality {get; set;} = string.Empty;

        public Clinician (int iD, string firstName, string lastName, DateOnly dob, 
                        string gmcNumber, string speciality) : base (iD, firstName, lastName, dob)
        {
            GMCNumber = speciality;
            Speciality = gmcNumber;
        }
    }
}