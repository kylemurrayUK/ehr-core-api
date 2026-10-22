using EHRCoreAPI.Models;

namespace EHRCoreAPI.Services;

public interface ITokenService
{
    public (string, DateTime) GenerateToken(User user);
}