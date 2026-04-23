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
    }
}