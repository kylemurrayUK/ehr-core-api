using Microsoft.AspNetCore.Authorization;
using EHRCoreAPI.Models;
using System.Security.Claims;
namespace EHRCoreAPI.Auth;

public class StatusChangeHandler : AuthorizationHandler<StatusChangeRequirement, AppointmentStatus>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, StatusChangeRequirement requirement, AppointmentStatus appointmentStatus)
    {
        bool isClinician = context.User.IsInRole(nameof(UserRole.Clinician));
        bool isAdmin = context.User.IsInRole(nameof(UserRole.Admin));

        bool allowed = appointmentStatus switch
        {
            AppointmentStatus.Completed      => isClinician,
            AppointmentStatus.Cancelled      => isAdmin,
            AppointmentStatus.Pending        => isClinician || isAdmin,
            AppointmentStatus.EnteredInError => isClinician || isAdmin,
            _ => false
        };

        if (allowed)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
        
    }
}