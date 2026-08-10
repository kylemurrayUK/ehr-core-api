using System.Data.Common;
using System.Runtime.CompilerServices;
using ehr_core_api.IntegrationTests;
using EHRCoreAPI.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == 
                    typeof(IDbContextOptionsConfiguration<ApiDbContext>));

            services.Remove(dbContextDescriptor);

            services.AddDbContext<ApiDbContext> (options => 
            options.UseSqlServer(@"Server=.\SQLEXPRESS;Database=EHRDb_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;ConnectRetryCount=0"));
        });
        

        builder.UseEnvironment("Development");
    }

    public void SeedDatabase()
    {
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            context.Database.EnsureCreated();
            TestSeedData.SeedTestData(context);
        }
    }

}