using EHRCoreAPI.Dtos;
using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories;

namespace EHRCoreAPI.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHashingService _passwordHasher;
    private readonly ITokenService _tokenService;
    public AuthService(IUserRepository userRepo, IPasswordHashingService passwordHasher, ITokenService tokenService)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }
    public async Task<AuthResult> LoginAsync(LoginRequestDTO loginRequest)
    {
        string emailToValidate = loginRequest.Email.ToLowerInvariant();
        AuthResult result = new AuthResult();

        var user = await _userRepo.GetUserByEmailAsync(emailToValidate);

        if (user == null)
        {
            result.LoginOutcome = LoginOutcome.InvalidCredentials;
            return result;
        }

        var passwordCheckResult = _passwordHasher.VerifyPasswordHash(loginRequest.Password, user.HashedPassword, user.HashSalt);

        if (!passwordCheckResult)
        {
            result.LoginOutcome = LoginOutcome.InvalidCredentials;
            return result;
        }

        if (user.Role == UserRole.Unassigned)
        {
            result.LoginOutcome = LoginOutcome.AccountInactive;
            return result;
        }

        var (token, expiryTime) = _tokenService.GenerateToken(user);

        result.LoginOutcome = LoginOutcome.Success;
        result.Token = token;
        result.ExpiresAt = expiryTime;

        return result;
    }
    
}