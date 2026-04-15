namespace EHRCoreAPI
{
    public class Patient : Person
    {
        public string NHSNumber {get; set;} = string.Empty;
        public string Address {get; set;} = string.Empty;
    }
}