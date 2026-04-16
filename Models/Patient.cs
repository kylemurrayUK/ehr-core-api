namespace EHRCoreAPI
{
    public class Patient : Person
    {
        public string NHSNumber {get; set;} = string.Empty;
        public string Address {get; set;} = string.Empty;

        public Patient (int iD, string firstName, string lastName, DateOnly dob, 
                        string nhsNumber, string address) : base (iD, firstName, lastName, dob)
        {
            NHSNumber = nhsNumber;
            Address = address;
        }
    }
}