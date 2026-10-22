using EHRCoreAPI.Models;

namespace EHRCoreAPI.Services;

public interface ITokenService
{
    public (string token, DateTime expiryTime) GenerateToken(User user);
}