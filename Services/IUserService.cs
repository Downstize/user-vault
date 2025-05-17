using UserVault.Dtos;
using UserVault.Models;

namespace UserVault.Services;

public interface IUserService
{
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByLoginAndPasswordAsync(string login, string password);
    Task<IEnumerable<User>> GetAllActiveAsync();
    Task<IEnumerable<User>> GetByAgeGreaterThanAsync(int age);
    Task<User?> CreateAsync(CreateUserDto dto, string createdBy);
    Task<bool> UpdateNameAsync(string login, string name, string updatedBy);
    Task<bool> UpdatePasswordAsync(string login, string password, string updatedBy);
    Task<bool> UpdateLoginAsync(string oldLogin, string newLogin, string updatedBy);
    Task<bool> SoftDeleteAsync(string login, string deletedBy);
    Task<bool> RestoreAsync(string login);
}