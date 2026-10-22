using EHRCoreAPI.Configuration;
using EHRCoreAPI.Models;
using Microsoft.Extensions.Options;

namespace EHRCoreAPI.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public string GenerateToken(User user)
    {
        
    }
}