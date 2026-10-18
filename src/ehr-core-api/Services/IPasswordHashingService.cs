namespace EHRCoreAPI.Services;

public interface IPasswordHashingService
{
    public (byte[] hashedPassword, byte[] hashSalt) CreatePasswordHash(string password);

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
}