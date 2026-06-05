using EHRCoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using EHRCoreAPI.Models;

namespace ehr_core_api.IntegrationTests;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=.\SQLEXPRESS;Database=EHRDb_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;ConnectRetryCount=0";

    public TestDatabaseFixture()
    {
        using (var context = CreateContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var clinicians = new List<Clinician>
            {
                new Clinician { FirstName = "Johhny", LastName = "Doc", Dob = new DateOnly(1950, 5, 11), GmcNumber = "0987654", Specialty = "Surgery" },
                new Clinician { FirstName = "William", LastName = "Murray", Dob = new DateOnly(1960, 11, 11), GmcNumber = "4567890", Specialty = "Cardiology" },
                new Clinician { FirstName = "Susan", LastName = "Murray", Dob = new DateOnly(1970, 8, 2), GmcNumber = "7869504", Specialty = "Gastrointestinal" },
                new Clinician { FirstName = "Margaret", LastName = "Neil", Dob = new DateOnly(1958, 10, 2), GmcNumber = "4059687", Specialty = "Pharmacology" }
            };
            context.Clinicians.AddRange(clinicians);

            var patients = new List<Patient>
            {
                new Patient { FirstName = "Kyle", LastName = "Murray", Dob = new DateOnly(1980, 6, 6), NhsNumber = "1234567", Address = "16 Bronson Close" },
                new Patient { FirstName = "Declan", LastName = "Mahoney", Dob = new DateOnly(1950, 10, 26), NhsNumber = "7654321", Address = "1 New York Street" },
                new Patient { FirstName = "Max", LastName = "Briggs", Dob = new DateOnly(1999, 5, 20), NhsNumber = "4536271", Address = "Somewhere in Leeds" },
                new Patient { FirstName = "Ali", LastName = "Grundy", Dob = new DateOnly(1997, 2, 18), NhsNumber = "6253417", Address = "Warwick Way" }
            };
            context.Patients.AddRange(patients);

            context.SaveChanges(); // patients and clinicians now have DB-assigned ids

            var appointments = new List<Appointment>
            {
                new Appointment { Patient = patients[0], Clinician = clinicians[1], Department = "Cardiology",       Status = AppointmentStatus.Pending,        AppointmentTime = new DateTime(2026, 6, 15, 9, 0, 0) },
                new Appointment { Patient = patients[1], Clinician = clinicians[0], Department = "Surgery",          Status = AppointmentStatus.Completed,      AppointmentTime = new DateTime(2026, 6, 16, 10, 30, 0) },
                new Appointment { Patient = patients[2], Clinician = clinicians[2], Department = "Gastrointestinal", Status = AppointmentStatus.Cancelled,      AppointmentTime = new DateTime(2026, 6, 17, 14, 0, 0) },
                new Appointment { Patient = patients[0], Clinician = clinicians[3], Department = "Pharmacology",     Status = AppointmentStatus.EnteredInError, AppointmentTime = new DateTime(2026, 6, 18, 11, 15, 0) }
            };
            context.Appointments.AddRange(appointments);

        context.SaveChanges();
        }
    }

    public ApiDbContext CreateContext()
        => new ApiDbContext(
            new DbContextOptionsBuilder<ApiDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}
