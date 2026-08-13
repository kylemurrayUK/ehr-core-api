using System.Data.Common;
using System.Runtime.CompilerServices;
using ehr_core_api.IntegrationTests;
using EHRCoreAPI.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private bool _seeded = false;
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

                return connection;
            });

            services.AddDbContext<ApiDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
        

        builder.UseEnvironment("Testing");
    }

    public void SeedDatabase()
    {
        if(!_seeded){
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            context.Database.EnsureCreated();
            TestSeedData.SeedTestData(context);
            _seeded = true;
        }
        }
    }

}