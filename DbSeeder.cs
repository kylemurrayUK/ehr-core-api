using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EHRCoreAPI
{
    public class DbSeeder
    {   

        public static void SeedData(ApiDbContext _db) 
        {
            if (!_db.Patients.Any())
            {  
                _db.Patients.AddRange
                (                            
                    new Patient { FirstName = "Kyle", LastName = "Murray", Dob = new DateOnly(1997, 6, 16), NhsNumber = "1234567", Address = "16 Lidgate Close"},
                    new Patient { FirstName = "Declan", LastName = "Mahoney", Dob = new DateOnly(1996, 9, 16), NhsNumber = "7654321", Address = "1 New York Street"},
                    new Patient { FirstName = "Max", LastName = "Briggs", Dob = new DateOnly(1999, 5, 20),NhsNumber = "4536271", Address = "Somewhere in Leeds"},
                    new Patient { FirstName = "Ali", LastName=  "Grundy", Dob =  new DateOnly(1997, 6, 15), NhsNumber = "6253417", Address = "Warwick Way" }       
                );
                _db.SaveChanges();
            }

            if (!_db.Clinicians.Any())
            {
                _db.Clinicians.AddRange
                (
                    new Clinician { FirstName =  "Johhny", LastName = "Doc", Dob = new DateOnly(1950, 5, 1), GmcNumber = "0987654", Specialty = "Surgery" },
                    new Clinician { FirstName = "William", LastName = "Murray", Dob = new DateOnly(1959, 12, 17), GmcNumber = "4567890", Specialty = "Cardiology" },
                    new Clinician { FirstName = "Susan", LastName = "Murray", Dob = new DateOnly(1958, 11, 9), GmcNumber =  "7869504", Specialty ="Gastrointestinal"},
                    new Clinician { FirstName =  "Margaret", LastName = "Neil", Dob = new DateOnly(1958, 10, 2), GmcNumber = "4059687", Specialty ="Pharmacology"}
                );
                _db.SaveChanges();
            }
        }
    }
}