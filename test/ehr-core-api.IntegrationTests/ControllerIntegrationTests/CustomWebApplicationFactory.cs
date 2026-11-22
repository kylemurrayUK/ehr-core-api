using System.Data.Common;
using ehr_core_api.IntegrationTests;
using EHRCoreAPI.Data;
using EHRCoreAPI.Models;
using EHRCoreAPI.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private bool _seeded = false;
    public DbConnection? Connection{get; set;}
    public (IReadOnlyList<Patient> patients, IReadOnlyList<Clinician> clinicians, IReadOnlyList<Appointment> appointments, IReadOnlyList<User> users) SeededData {get; private set;}

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(
                d => d.ServiceType == 
                    typeof(IDbContextOptionsConfiguration<ApiDbContext>));

            services.Remove(dbContextDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                Connection = connection;
                return connection;
            });

            

            services.AddDbContext<ApiDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        builder.UseSetting("Jwt:Issuer", "Test Issuer");
        builder.UseSetting("Jwt:Audience", "Test Audience");
        builder.UseSetting("Jwt:ExpiryMinutes", "15");
        builder.UseSetting("Jwt:Key", Convert.ToBase64String(new byte[32]));

        builder.UseEnvironment("Testing");
    }

    public void SeedDatabase()
    {
        if(!_seeded){
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            context.Database.EnsureCreated();
            SeededData = TestSeedData.SeedTestData(context);
            
            _seeded = true;
        }
        }
    }

    public HttpClient CreateClientAuthenticatedAs(UserRole role, int? clinicianId = null)
    {
        User testUser = new()
        {
            Id = 999,
            Email = "testuserfortoken@email.com",
            Role = role,
            ClinicianId = clinicianId
        };

        var tokenService = Services.GetRequiredService<ITokenService>();
        var token = tokenService.GenerateToken(testUser);

        var client = CreateClient();

        client.DefaultRequestHeaders.Authorization = 
                                    new AuthenticationHeaderValue("Bearer", token.token);

        return client;
    }

}