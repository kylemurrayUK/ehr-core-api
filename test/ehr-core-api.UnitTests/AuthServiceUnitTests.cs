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

    [Fact]
    public async Task LoginAsync_IncorrectPassword_ReturnLoginCredentialsInvalid()
    {
        // Arrange
        string email = "validemail@email.com";
        string incorrectPassword = "IncorrectPassword";


        var loginRequest = new LoginRequestDTO
        {
            Email = email, 
            Password = incorrectPassword
        };
        var testUser = new User
        {
            Email = email,
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Admin,
        };

        _mockUserRepo.Setup(mock => mock.GetUserByEmailAsync(email))
        .ReturnsAsync(testUser);
        _mockPasswordHasher.Setup(mock => mock.VerifyPasswordHash(It.IsAny<string>()
            ,It.IsAny<byte[]>()
            ,It.IsAny<byte[]>()
        )).Returns(false);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Equal(LoginOutcome.InvalidCredentials, result.LoginOutcome);

        _mockPasswordHasher.Verify(mock => mock.VerifyPasswordHash(
            It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())
            , Times.Once);
        _mockTokenService.Verify(mock => mock.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_UnassignedUser_ReturnAccountInactive()
    {
        // Arrange
        string email = "validemail@email.com";
        string password = "CorrectPassword";


        var loginRequest = new LoginRequestDTO
        {
            Email = email, 
            Password = password
        };
        var testUser = new User
        {
            Email = email,
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Unassigned,
        };

        _mockUserRepo.Setup(mock => mock.GetUserByEmailAsync(email))
        .ReturnsAsync(testUser);
        _mockPasswordHasher.Setup(mock => mock.VerifyPasswordHash(It.IsAny<string>()
            ,It.IsAny<byte[]>()
            ,It.IsAny<byte[]>()
        )).Returns(true);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Equal(LoginOutcome.AccountInactive, result.LoginOutcome);

        _mockPasswordHasher.Verify(mock => mock.VerifyPasswordHash(
            It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())
            , Times.Once);
        _mockTokenService.Verify(mock => mock.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CorrectLoginDetails_ReturnToken()
    {
        // Arrange
        string email = "validemail@email.com";
        string password = "CorrectPassword";

        var loginRequest = new LoginRequestDTO
        {
            Email = email, 
            Password = password
        };
        var testUser = new User
        {
            Email = email,
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Admin,
        };
        string testToken = "test-token-123";
        DateTime testExpiresAt = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc); 

        _mockUserRepo.Setup(mock => mock.GetUserByEmailAsync(email))
        .ReturnsAsync(testUser);
        _mockPasswordHasher.Setup(mock => mock.VerifyPasswordHash(It.IsAny<string>()
            ,It.IsAny<byte[]>()
            ,It.IsAny<byte[]>()
        )).Returns(true);
        _mockTokenService.Setup(mock => mock.GenerateToken(testUser)).Returns((testToken, testExpiresAt));

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Equal(LoginOutcome.Success, result.LoginOutcome);
        Assert.Equal(testToken, result.Token);
        Assert.Equal(testExpiresAt, result.ExpiresAt);

        _mockPasswordHasher.Verify(mock => mock.VerifyPasswordHash(
            It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())
            , Times.Once);
        _mockTokenService.Verify(mock => mock.GenerateToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_MixedCaseEmail_LowercaseLookedUp()
    {
        // Arrange
        string mixedCaseEmail = "Test@Email.Com";
        string lowerCaseEmail = "test@email.com";

        var loginRequest = new LoginRequestDTO
        {
            Email = mixedCaseEmail,
            Password = "randomPass"
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        _mockUserRepo.Verify(mock => mock.GetUserByEmailAsync(lowerCaseEmail), Times.Once);
    }
}