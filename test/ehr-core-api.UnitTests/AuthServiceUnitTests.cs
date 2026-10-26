using EHRCoreAPI.Dtos;
using EHRCoreAPI.Models;
using EHRCoreAPI.Repositories;
using EHRCoreAPI.Services;
using EHRCoreAPI.Services.Implementations;

namespace EHRCoreAPI.Tests;

public class AuthServiceUnitTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IPasswordHashingService> _mockPasswordHasher;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly AuthService _authService;

    public AuthServiceUnitTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHashingService>();
        _mockTokenService = new Mock<ITokenService>();

        _authService = new AuthService(
            _mockUserRepo.Object,
            _mockPasswordHasher.Object,
            _mockTokenService.Object
        );
    }

    [Fact]
    public async Task LoginAsync_IncorrectEmailAddress_ReturnLoginCredentialsInvalid()
    {
        // Arrange
        string invalidEmail = "notvalidemail@email.com";
        var loginRequest = new LoginRequestDTO
        {
            Email = invalidEmail, 
            Password = "IncorrectPassword"
        };

        _mockUserRepo.Setup(mock => mock.GetUserByEmailAsync(invalidEmail))
        .ReturnsAsync((User?)null);


        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Equal(LoginOutcome.InvalidCredentials, result.LoginOutcome);

        _mockPasswordHasher.Verify(mock => mock.VerifyPasswordHash(
            It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())
            , Times.Never);
        _mockTokenService.Verify(mock => mock.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}