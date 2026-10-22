namespace EHRCoreAPI.Services;

public interface ITokenService
{
    public (byte[] hashedPassword, byte[] hashSalt) GenerateToken(string password);

}