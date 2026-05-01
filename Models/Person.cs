using System.ComponentModel.DataAnnotations.Schema;

namespace EHRCoreAPI
{
    // Inheriting for sucha a generic base class isn't conventional but I am adding it here 
    // to get some exposure to inheritance as it doesn't come up elsewhere in my roadmap.
   public class Person
    {
        public int Id {get; set;} 
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public DateOnly Dob {get; set;}


    } 
}