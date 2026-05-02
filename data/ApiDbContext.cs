using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

namespace EHRCoreAPI
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext (DbContextOptions<ApiDbContext> options) : base(options)
        {
            
        }

        public DbSet<Appointment> Appointments { get; set;}
        public DbSet<Patient> Patients {get; set;}
        public DbSet<Clinician> Clinicians {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity("EHRCoreAPI.Appointment", b =>
                {
                    b.HasOne("EHRCoreAPI.Clinician", "Clinician")
                        .WithMany()
                        .HasForeignKey("ClinicianId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EHRCoreAPI.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Clinician");

                    b.Navigation("Patient");
                });
        }
    }
}