using EHRCoreAPI.Services.Implementations;
using EHRCoreAPI.Services;
using EHRCoreAPI.Repositories.Implementations;
using EHRCoreAPI.Repositories;
using EHRCoreAPI.Data;
using EHRCoreAPI;
using EHRCoreAPI.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using EHRCoreAPI.Auth;
using EHRCoreAPI.Models;

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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();
builder.Services.AddSingleton<IAuthorizationHandler, StatusChangeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CliniciansOwnAppointmentHandler>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                    ?? throw new InvalidOperationException("JWT settings are missing");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                    .AddJwtBearer(options => {options.TokenValidationParameters = new TokenValidationParameters
                                    {
                                        ValidateIssuer = true,
                                        ValidIssuer = jwtSettings.Issuer,

                                        ValidateAudience = true,
                                        ValidAudience = jwtSettings.Audience,

                                        ValidateIssuerSigningKey = true,
                                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.Key)),

                                        ValidateLifetime = true,
                                        ClockSkew = TimeSpan.FromSeconds(2)
                                    };});

var requireRolePolicy = new AuthorizationPolicyBuilder()
    .RequireRole(nameof(UserRole.Admin), nameof(UserRole.Clinician))
    .Build();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(PolicyNames.AdminAndClinician, requireRolePolicy)
    .AddPolicy(PolicyNames.ChangeAppointmentStatus, policy =>
        policy.Requirements.Add(new StatusChangeRequirement()))
    .AddPolicy(PolicyNames.CliniciansOwnAppointments, policy =>
        policy.Requirements.Add(new CliniciansOwnAppointmentRequirement()));



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
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHashingService>();
        DbSeeder.SeedData(context, passwordHasher);
    }
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

