using EHRCoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ehr_core_api.IntegrationTests;

[Collection("Integration tests")]
public class UserTableConstraintTests
{
    private readonly TestDatabaseFixture _fixture;

    public UserTableConstraintTests()
    {
        _fixture = new TestDatabaseFixture();
    }

    [Fact]
    public async Task UniqueEmailConstraint_AddUserWithSameEmail_ThrowsDbUpdateException()
    {
        // Arrange
        string duplicateEmail = _fixture.SeededData.users[0].Email;

        using var context = _fixture.CreateContext();
        context.Users.Add(new User
        {
            Email = duplicateEmail,
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Admin
        });

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task UniqueClinicianConstraint_AddUserWithSameClinician_ThrowsDbUpdateException()
    {
        // Arrange
        int? duplicateClinicianId = _fixture.SeededData.users
            .First(u => u.Role == UserRole.Clinician)
            .ClinicianId;


        Assert.NotNull(duplicateClinicianId);

        using var context = _fixture.CreateContext();
        context.Users.Add(new User
        {
            Email = "uniquetestemail@email.com",
            HashedPassword = [],
            HashSalt = [],
            Role = UserRole.Clinician,
            ClinicianId = duplicateClinicianId
        });

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}