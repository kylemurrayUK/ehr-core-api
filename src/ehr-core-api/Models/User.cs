using System.ComponentModel.DataAnnotations;

namespace EHRCoreAPI.Models
{
    // User model for implementing authorization and authentication
   public class User
    {
        public int Id {get; set;}  
        [Required]
        public string Email {get; set;} = string.Empty;
        [Required]
        public byte[] HashedPassword {get; set;} = [];
        [Required]
        public byte[] HashSalt {get; set;} = [];
        public UserRole Role {get; set;}
        public int? ClinicianId {get; set;}
        public Clinician? Clinician {get; set;}
    } 
}