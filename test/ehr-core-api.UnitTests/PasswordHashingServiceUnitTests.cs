using EHRCoreAPI.Services.Implementations;

namespace ehr_core_api.Tests;

public class PasswordHashingServiceUnitTests
{
    private readonly PasswordHashingService _passwordHashingService;
    public PasswordHashingServiceUnitTests()
    {
        _passwordHashingService = new PasswordHashingService();
    }


    [Fact]
    public void VerifyPasswordHash_CorrectPassword_ReturnsTrue()
    {
        //Arrange
        string password = "Test@678";
        var (hash, salt) = _passwordHashingService.CreatePasswordHash(password);

        //Act
        bool comparisonResult = _passwordHashingService.VerifyPasswordHash(password, hash, salt);
    
        //Assert
        Assert.True(comparisonResult);
    }

    [Fact]
    public void VerifyPasswordHash_IncorrectPassword_ReturnsFalse()
    {
        //Arrange
        string password = "Test@678";
        string incorrectPassword = "test@678";

        var (hash, salt) = _passwordHashingService.CreatePasswordHash(password);

        //Act
        bool comparisonResult = _passwordHashingService.VerifyPasswordHash(incorrectPassword, hash, salt);
    
        //Assert
        Assert.False(comparisonResult);
    }

    [Fact]
    public void CreatePasswordHash_SamePasswordHashedTwice_HashAndSaltNotEqual()
    {
        //Arrange
        string password = "Test@678";

        //Act
        var (hash1, salt1) = _passwordHashingService.CreatePasswordHash(password);
        var (hash2, salt2) = _passwordHashingService.CreatePasswordHash(password);
    
        //Assert
        Assert.NotEqual(hash1, hash2);
        Assert.NotEqual(salt1, salt2);
    }

}