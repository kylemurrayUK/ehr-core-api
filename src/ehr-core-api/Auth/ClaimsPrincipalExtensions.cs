using System.Security.Claims;

namespace EHRCoreAPI.Auth;

public static class ClaimsPrincipalExtensions
{
    public static int? GetClinicianId(this ClaimsPrincipal user)
    {
        var clinicianIdClaim = user.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.ClinicianId);

        if (clinicianIdClaim == null)
            return null;

        return int.TryParse(clinicianIdClaim.Value, out var clinicianId) ? clinicianId : null;
    }
}