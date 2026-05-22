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
            modelBuilder.Entity<Appointment>().HasOne(a => a.Clinician)
                                              .WithMany()
                                              .HasForeignKey(a => a.ClinicianId)
                                              .OnDelete(DeleteBehavior.Restrict)
                                              .IsRequired();
            modelBuilder.Entity<Appointment>().HasOne(a => a.Patient)
                                              .WithMany()
                                              .HasForeignKey(a => a.PatientId)
                                              .OnDelete(DeleteBehavior.Restrict)
                                              .IsRequired();
            modelBuilder.Entity<Appointment>().Property(a => a.Department)
                                              .IsRequired()
                                              .HasMaxLength(30);



            modelBuilder.Entity<Patient>().Property(p => p.FirstName)
                                          .IsRequired()
                                          .HasMaxLength(100);
            modelBuilder.Entity<Patient>().Property(p => p.LastName)
                                          .IsRequired()
                                          .HasMaxLength(100);
            modelBuilder.Entity<Patient>().Property(p => p.Dob)
                                          .IsRequired();
            modelBuilder.Entity<Patient>().Property(p => p.Address)
                                          .IsRequired()
                                          .HasMaxLength(200);
            modelBuilder.Entity<Patient>().Property(p => p.NhsNumber)
                                          .IsRequired()
                                          .HasMaxLength(10);            
            modelBuilder.Entity<Patient>().HasIndex(p => p.NhsNumber)
                                          .IsUnique(); 

            modelBuilder.Entity<Clinician>().Property(p => p.FirstName)
                                          .IsRequired()
                                          .HasMaxLength(100);
            modelBuilder.Entity<Clinician>().Property(p => p.LastName)
                                          .IsRequired()
                                          .HasMaxLength(100);
            modelBuilder.Entity<Clinician>().Property(p => p.Dob)
                                          .IsRequired();
            modelBuilder.Entity<Clinician>().Property(c => c.Specialty)
                                            .IsRequired()
                                            .HasMaxLength(30);                                        
            modelBuilder.Entity<Clinician>().Property(c => c.GmcNumber)
                                          .IsRequired()
                                          .HasMaxLength(7);            
            modelBuilder.Entity<Clinician>().HasIndex(c => c.GmcNumber)
                                          .IsUnique();                          
        }
    }
}