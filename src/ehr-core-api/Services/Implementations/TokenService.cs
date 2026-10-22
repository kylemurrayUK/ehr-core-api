using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EHRCoreAPI.Configuration;
using EHRCoreAPI.Models;
using EHRCoreAPI.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace EHRCoreAPI.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public (string, DateTime) GenerateToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Key));

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if(user.ClinicianId != null)
        {
            claims.Add(new Claim(CustomClaimTypes.ClinicianId, user.ClinicianId.Value.ToString()));
        }

        var expiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var tokenDescriptor = new JwtSecurityToken
        (
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiryTime,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.WriteToken(tokenDescriptor);

        return (token, expiryTime);

    }
}