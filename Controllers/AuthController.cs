using Microsoft.AspNetCore.Mvc;
using UserVault.Dtos;
using UserVault.Services;

namespace UserVault.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(IUserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequestDto dto)
    {
        var user = await _userService.GetByLoginAndPasswordAsync(dto.Login, dto.Password);
        if (user == null || user.RevokedOn != null)
            return Unauthorized("Неверный логин или пароль");

        var token = _jwtService.GenerateToken(user);
        return Ok(new { token });
    }
}