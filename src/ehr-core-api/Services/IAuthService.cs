using EHRCoreAPI.Dtos;
using EHRCoreAPI.Models;

namespace EHRCoreAPI.Services;

public interface IAuthService
{
    Task<AuthResult> Login(LoginRequestDTO loginRequest);
}