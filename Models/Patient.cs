namespace EHRCoreAPI
{
    public class Patient : Person
    {
        public string NhsNumber {get; set;} = string.Empty;
        public string Address {get; set;} = string.Empty;

        public Patient (int Id, string firstName, string lastName, DateOnly dob, 
                        string nhsNumber, string address) : base (Id, firstName, lastName, dob)
        {
            NhsNumber = nhsNumber;
            Address = address;
        }
    }
}