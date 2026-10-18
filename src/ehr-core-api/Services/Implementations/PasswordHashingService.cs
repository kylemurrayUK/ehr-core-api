using System.Security.Cryptography;

namespace EHRCoreAPI.Services.Implementations;

public class PasswordHashingService : IPasswordHashingService
{
    const int HashingIterations = 600000;
    const int SaltByteLength = 16;
    const int PasswordHashByteLength = 32;
    static readonly HashAlgorithmName algorithmName = HashAlgorithmName.SHA256;


    public (byte[] hashedPassword, byte[] hashSalt) CreatePasswordHash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltByteLength);

        byte[] passwordHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, HashingIterations, algorithmName, PasswordHashByteLength);

        return (passwordHash, salt);
    }
}