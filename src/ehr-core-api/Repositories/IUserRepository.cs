using EHRCoreAPI.Models;

namespace EHRCoreAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
}