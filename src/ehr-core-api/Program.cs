
using System.Data.Common;
using EHRCoreAPI.Repositories.Implementations;
using EHRCoreAPI.Repositories;
using EHRCoreAPI.Data;
using EHRCoreAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddDbContext<ApiDbContext> (options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IClinicianRepository, ClinicianRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        DbSeeder.SeedData(context);
    }
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

