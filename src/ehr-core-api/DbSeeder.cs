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
                    new Patient { FirstName = "Kyle", LastName = "Murray", Dob = new DateOnly(1980, 6, 6), NhsNumber = "1234567", Address = "16 Lidgate Close"},
                    new Patient { FirstName = "Declan", LastName = "Mahoney", Dob = new DateOnly(1950, 10, 26), NhsNumber = "7654321", Address = "1 New York Street"},
                    new Patient { FirstName = "Max", LastName = "Briggs", Dob = new DateOnly(1999, 5, 20),NhsNumber = "4536271", Address = "Somewhere in Leeds"},
                    new Patient { FirstName = "Ali", LastName=  "Grundy", Dob =  new DateOnly(1997, 2, 18), NhsNumber = "6253417", Address = "Warwick Way" }       
                );
                _db.SaveChanges();
            }

            if (!_db.Clinicians.Any())
            {
                _db.Clinicians.AddRange
                (
                    new Clinician { FirstName =  "Johhny", LastName = "Doc", Dob = new DateOnly(1950, 5, 11), GmcNumber = "0987654", Specialty = "Surgery" },
                    new Clinician { FirstName = "William", LastName = "Murray", Dob = new DateOnly(1960, 11, 11), GmcNumber = "4567890", Specialty = "Cardiology" },
                    new Clinician { FirstName = "Susan", LastName = "Murray", Dob = new DateOnly(1970, 8, 2), GmcNumber =  "7869504", Specialty ="Gastrointestinal"},
                    new Clinician { FirstName =  "Margaret", LastName = "Neil", Dob = new DateOnly(1958, 10, 2), GmcNumber = "4059687", Specialty ="Pharmacology"}
                );
                _db.SaveChanges();
            }
        }
    }
}