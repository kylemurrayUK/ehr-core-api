using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EHRCoreAPI.Auth;
using EHRCoreAPI.Configuration;
using EHRCoreAPI.Models;
using EHRCoreAPI.Services.Implementations;
using Microsoft.Extensions.Options;

namespace EHRCoreAPI.Tests;

public class TokenServiceUnitTests
{
    private readonly JwtSettings _testTokenSettings;
    private readonly TokenService _tokenService;
    public TokenServiceUnitTests()
    {
        _testTokenSettings = new JwtSettings
        {
            Audience = "TestAudience",
            Issuer = "TestIssuer",
            ExpiryMinutes = 15,
            Key = Convert.ToBase64String(new byte[32])
        };

        var options = Options.Create(_testTokenSettings);
        _tokenService = new TokenService(options);
    }

    [Fact]
    public void GenerateToken_UserIsClinician_ReturnsClinicianDetails()
    {
        // Arrange
        var testUser = new User
        {
            Email = "testclinician@email.com",
            ClinicianId = 99,
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Clinician
        };

        // Act
        var result = _tokenService.GenerateToken(testUser);


        // Assert
        var readableToken = new JwtSecurityTokenHandler().ReadJwtToken(result.token);
        var clinicianIdInClaim = readableToken.Claims.First(c => c.Type == CustomClaimTypes.ClinicianId);
        
        Assert.Equal("99", clinicianIdInClaim.Value);
    }

    [Fact]
    public void GenerateToken_UserIsJustAdmin_ReturnsTokenWithNoClinician()
    {
        // Arrange
        var testUser = new User
        {
            Email = "testadmin@email.com",
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Admin
        };

        // Act
        var result = _tokenService.GenerateToken(testUser);


        // Assert
        var readableToken = new JwtSecurityTokenHandler().ReadJwtToken(result.token);
        
        Assert.DoesNotContain(readableToken.Claims, c => c.Type == CustomClaimTypes.ClinicianId);
    }

    [Fact]
    public void GenerateToken_UserIsAdmin_ReturnsCorrectIdentityAndRole()
    {
        // Arrange
        var testUser = new User
        {
            Id = 47,
            Email = "testadmin@email.com",
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Admin
        };

        // Act
        var result = _tokenService.GenerateToken(testUser);


        // Assert
        var readableToken = new JwtSecurityTokenHandler().ReadJwtToken(result.token);
        var subClaim = readableToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
        var roleClaim = readableToken.Claims.First(c => c.Type == ClaimTypes.Role);
        
        Assert.Equal("47", subClaim.Value);
        Assert.Equal("Admin", roleClaim.Value);
    }

    [Fact]
    public void GenerateToken_StandardRequest_ReturnsCorrectExpiryTime()
    {
        // Arrange
        var testUser = new User
        {
            Id = 47,
            Email = "testadmin@email.com",
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Admin
        };

        DateTime timeBeforeRequest = DateTime.UtcNow;
        // Act
        var result = _tokenService.GenerateToken(testUser);

        DateTime timeAfterRequest = DateTime.UtcNow;


        // Assert
        var readableToken = new JwtSecurityTokenHandler().ReadJwtToken(result.token);
        var expiry = readableToken.ValidTo;
        
        // checking expiry date falls within 15 mins (Expiry time in settings). Minus a few seconds on before to account for rounding 
        Assert.InRange(expiry, 
                       timeBeforeRequest.AddMinutes(_testTokenSettings.ExpiryMinutes).AddSeconds(-5), 
                       timeAfterRequest.AddMinutes(_testTokenSettings.ExpiryMinutes));

    }
}