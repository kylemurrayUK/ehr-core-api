using EHRCoreAPI.Models;
using EHRCoreAPI.Data;
using EHRCoreAPI.Services;

namespace EHRCoreAPI
{
    public class DbSeeder
    {   

        public static void SeedData(ApiDbContext db, IPasswordHashingService passwordHasher) 
        {
            if (!db.Patients.Any())
            {  
                db.Patients.AddRange
                (                            
                    new Patient { FirstName = "Kyle", LastName = "Murray", Dob = new DateOnly(1980, 6, 6), NhsNumber = "1234567", Address = "16 Lidgate Close"},
                    new Patient { FirstName = "Declan", LastName = "Mahoney", Dob = new DateOnly(1950, 10, 26), NhsNumber = "7654321", Address = "1 New York Street"},
                    new Patient { FirstName = "Max", LastName = "Briggs", Dob = new DateOnly(1999, 5, 20),NhsNumber = "4536271", Address = "Somewhere in Leeds"},
                    new Patient { FirstName = "Ali", LastName=  "Grundy", Dob =  new DateOnly(1997, 2, 18), NhsNumber = "6253417", Address = "Warwick Way" }       
                );
                db.SaveChanges();
            }

            if (!db.Clinicians.Any())
            {
                db.Clinicians.AddRange
                (
                    new Clinician { FirstName =  "Johhny", LastName = "Doc", Dob = new DateOnly(1950, 5, 11), GmcNumber = "0987654", Specialty = "Surgery" },
                    new Clinician { FirstName = "William", LastName = "Murray", Dob = new DateOnly(1960, 11, 11), GmcNumber = "4567890", Specialty = "Cardiology" },
                    new Clinician { FirstName = "Susan", LastName = "Murray", Dob = new DateOnly(1970, 8, 2), GmcNumber =  "7869504", Specialty ="Gastrointestinal"},
                    new Clinician { FirstName =  "Margaret", LastName = "Neil", Dob = new DateOnly(1958, 10, 2), GmcNumber = "4059687", Specialty ="Pharmacology"}
                );
                db.SaveChanges();
            }

            if (!db.Users.Any())
            {

                string adminUserPassword = "Test@678";
                string clinicianUserPassword = "Clinic@89";
                string noRoleAssignedPassword = "NoRole!890";

                var (adminHash, adminSalt) = passwordHasher.CreatePasswordHash(adminUserPassword);
                var (clinicianHash, clinicianSalt) = passwordHasher.CreatePasswordHash(clinicianUserPassword);
                var (noRoleHash, noRoleSalt) = passwordHasher.CreatePasswordHash(noRoleAssignedPassword);


                db.Users.AddRange
                (
                    new User { Email = "adminemail@test.com", HashedPassword = adminHash, HashSalt = adminSalt, Role = UserRole.Admin},
                    new User { Email = "clinicianemail@test.com", HashedPassword = clinicianHash, HashSalt = clinicianSalt, Role = UserRole.Clinician, Clinician =db.Clinicians.First(c => c.GmcNumber == "0987654")},
                    // Left role intentionally blank here to show it will default to no role as a fail safe
                    new User { Email = "noroleemail@test.com", HashedPassword = noRoleHash, HashSalt = noRoleSalt}
                );
                db.SaveChanges();
            }
        }
    }
}