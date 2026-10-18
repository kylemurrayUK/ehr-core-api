using System.Security.Cryptography;

namespace EHRCoreAPI.Services.Implementations;

public class PasswordHashingService : IPasswordHashingService
{
    const int HashingIterations = 600000;
    const int SaltByteLength = 16;
    const int PasswordHashByteLength = 32;
    static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA256;


    public (byte[] hashedPassword, byte[] hashSalt) CreatePasswordHash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltByteLength);

        byte[] passwordHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, HashingIterations, AlgorithmName, PasswordHashByteLength);

        return (passwordHash, salt);
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(password, storedSalt, HashingIterations, AlgorithmName, PasswordHashByteLength);
        
        return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
    }
}