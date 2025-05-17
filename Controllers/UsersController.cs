using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserVault.Dtos;
using UserVault.Models;
using UserVault.Services;

namespace UserVault.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UsersController> _logger;

    private string CurrentLogin => User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
    private bool IsAdminRole => User.IsInRole("Admin");

    public UsersController(IUserService service, ILogger<UsersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        _logger.LogInformation("[{User}] создаёт нового пользователя: {Login}", CurrentLogin, dto.Login);

        var result = await _service.CreateAsync(dto, CurrentLogin);
        return result is null
            ? BadRequest("Пользователь с данным логином уже существует")
            : Ok(result.Guid);
    }
    
    [HttpGet("validate")]
    public async Task<IActionResult> ValidateSelf([FromQuery] string login, [FromQuery] string password)
    {
        if (!string.Equals(CurrentLogin, login, StringComparison.OrdinalIgnoreCase))
            return Forbid("Можно получить только свои данные");

        var user = await _service.GetByLoginAndPasswordAsync(login, password);
        return user == null ? Unauthorized("Пользователь не найден или деактивирован") : Ok(ToDto(user));
    }

    [HttpPut("update/name/{login}")]
    public async Task<IActionResult> UpdateName(string login, [FromQuery] string name)
    {
        if (!await IsSelfOrAdmin(login))
            return Forbid("Недостаточно прав для изменения имени");

        _logger.LogInformation("[{User}] обновляет имя пользователя {Login}", CurrentLogin, login);
        var updated = await _service.UpdateNameAsync(login, name, CurrentLogin);
        return updated ? Ok() : NotFound("Пользователь не найден");
    }

    [HttpPut("update/password/{login}")]
    public async Task<IActionResult> UpdatePassword(string login, [FromQuery] string password)
    {
        if (!await IsSelfOrAdmin(login))
            return Forbid("Недостаточно прав для смены пароля");

        _logger.LogInformation("[{User}] меняет пароль пользователя {Login}", CurrentLogin, login);
        var updated = await _service.UpdatePasswordAsync(login, password, CurrentLogin);
        return updated ? Ok() : NotFound("Пользователь не найден");
    }

    [HttpPut("update/login/{oldLogin}")]
    public async Task<IActionResult> UpdateLogin(string oldLogin, [FromQuery] string newLogin)
    {
        if (!await IsSelfOrAdmin(oldLogin))
            return Forbid("Недостаточно прав для изменения логина");

        _logger.LogInformation("[{User}] меняет логин с {OldLogin} на {NewLogin}", CurrentLogin, oldLogin, newLogin);
        var updated = await _service.UpdateLoginAsync(oldLogin, newLogin, CurrentLogin);
        return updated ? Ok() : Conflict("Новый логин уже занят или старый логин не найден");
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("[{User}] запрашивает список всех активных пользователей", CurrentLogin);
        var users = await _service.GetAllActiveAsync();
        return Ok(users.Select(ToDto));
    }

    [HttpGet("by-login/{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByLogin(string login)
    {
        _logger.LogInformation("[{User}] ищет пользователя по логину: {Login}", CurrentLogin, login);
        var user = await _service.GetByLoginAsync(login);
        return user == null
            ? NotFound("Пользователь не найден")
            : Ok(ToDto(user));
    }

    [HttpGet("older-than/{age}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOlderThan(int age)
    {
        _logger.LogInformation("[{User}] ищет пользователей старше {Age}", CurrentLogin, age);
        var users = await _service.GetByAgeGreaterThanAsync(age);
        return Ok(users.Select(ToDto));
    }

    [HttpDelete("{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDelete(string login)
    {
        _logger.LogWarning("[{User}] удаляет пользователя {Login}", CurrentLogin, login);
        var deleted = await _service.SoftDeleteAsync(login, CurrentLogin);
        return deleted ? Ok() : NotFound("Пользователь не найден или уже удалён");
    }

    [HttpPut("restore/{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Restore(string login)
    {
        _logger.LogInformation("[{User}] восстанавливает пользователя {Login}", CurrentLogin, login);
        var restored = await _service.RestoreAsync(login);
        return restored ? Ok() : NotFound("Пользователь не найден или уже активен");
    }

    private async Task<bool> IsSelfOrAdmin(string login)
    {
        if (IsAdminRole) return true;
        return string.Equals(CurrentLogin, login, StringComparison.OrdinalIgnoreCase);
    }

    private static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Login = user.Login,
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            IsActive = user.RevokedOn == null
        };
    }
}
