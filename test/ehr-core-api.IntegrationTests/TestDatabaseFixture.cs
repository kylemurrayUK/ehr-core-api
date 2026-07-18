using EHRCoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using EHRCoreAPI.Models;
using Microsoft.Identity.Client;


namespace ehr_core_api.IntegrationTests;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=.\SQLEXPRESS;Database=EHRDb_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;ConnectRetryCount=0";
    public  (IReadOnlyList<Patient> patients, IReadOnlyList<Clinician> clinicians, IReadOnlyList<Appointment> appointments) SeededData {get;}
    
    public TestDatabaseFixture()
    {

        using (var context = CreateContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SeededData = TestSeedData.SeedTestData(context);
        }
        
    }

    public ApiDbContext CreateContext()
        => new ApiDbContext(
            new DbContextOptionsBuilder<ApiDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}
