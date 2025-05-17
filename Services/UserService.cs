using Microsoft.EntityFrameworkCore;
using UserVault.Data;
using UserVault.Dtos;
using UserVault.Models;

namespace UserVault.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<User?> GetByLoginAsync(string login) =>
        await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login);

    public async Task<User?> GetByLoginAndPasswordAsync(string login, string password) =>
        await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login && u.Password == password && u.RevokedOn == null);

    public async Task<IEnumerable<User>> GetAllActiveAsync() =>
        await _db.Users
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<User>> GetByAgeGreaterThanAsync(int age)
    {
        var cutoff = DateTime.UtcNow.AddYears(-age);
        return await _db.Users
            .Where(u => u.RevokedOn == null && u.Birthday != null && u.Birthday <= cutoff)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> CreateAsync(CreateUserDto dto, string createdBy)
    {
        var exists = await _db.Users.AnyAsync(u => u.Login == dto.Login);
        if (exists) return null;

        var user = new User
        {
            Login = dto.Login,
            Password = dto.Password,
            Name = dto.Name,
            Gender = dto.Gender,
            Birthday = dto.Birthday,
            Admin = dto.Admin,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateNameAsync(string login, string name, string updatedBy)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null || user.RevokedOn != null) return false;
        user.Name = name;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = updatedBy;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(string login, string password, string updatedBy)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null || user.RevokedOn != null) return false;
        user.Password = password;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = updatedBy;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateLoginAsync(string oldLogin, string newLogin, string updatedBy)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == oldLogin);
        if (user == null || user.RevokedOn != null) return false;
        var exists = await _db.Users.AnyAsync(u => u.Login == newLogin);
        if (exists) return false;
        user.Login = newLogin;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = updatedBy;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(string login, string deletedBy)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null || user.RevokedOn != null) return false;
        user.RevokedOn = DateTime.UtcNow;
        user.RevokedBy = deletedBy;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(string login)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null || user.RevokedOn == null) return false;
        user.RevokedOn = null;
        user.RevokedBy = null;
        await _db.SaveChangesAsync();
        return true;
    }
}
