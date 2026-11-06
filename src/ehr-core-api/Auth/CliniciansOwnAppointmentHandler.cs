using EHRCoreAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace EHRCoreAPI.Auth;

public class CliniciansOwnAppointmentHandler : AuthorizationHandler<CliniciansOwnAppointmentRequirement, Appointment>
{
    protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context, CliniciansOwnAppointmentRequirement requirement, Appointment appointment)
    {
        var userClinicianIdClaim = context.User.FindFirst(CustomClaimTypes.ClinicianId);
        var isAdmin = context.User.IsInRole(nameof(UserRole.Admin));

        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask; 
        }
        if (userClinicianIdClaim == null)
        {
            return Task.CompletedTask;    
        }
        if (userClinicianIdClaim.Value == appointment.ClinicianId.ToString())
        {
            context.Succeed(requirement);
            return Task.CompletedTask; 
        }

        return Task.CompletedTask;
    }
}