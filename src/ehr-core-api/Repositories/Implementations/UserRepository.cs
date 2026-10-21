using EHRCoreAPI.Models;
using EHRCoreAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace EHRCoreAPI.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly ApiDbContext _db;
    public UserRepository(ApiDbContext db)
    {
        _db = db;
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}