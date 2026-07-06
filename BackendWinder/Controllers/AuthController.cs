using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BackendWinder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly OutsideContext _outsideContext;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        OutsideContext outsideContext,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _outsideContext = outsideContext;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginDto.Login, loginDto.Password);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(new LoginResponseDto
        {
            UserId = result.UserId!.Value,
            Login = result.Login!,
            FullName = result.FullName!,
            Role = result.Role!,
            Token = result.Token!,
            ExpiresAt = DateTime.UtcNow.AddMinutes(480)
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Пользователь не авторизован" });

            var userId = int.Parse(userIdClaim.Value);

            var user = await _outsideContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.Login,
                    u.FullName,
                    u.Role,
                    u.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении текущего пользователя");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
}