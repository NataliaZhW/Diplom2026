using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendWinder.Controllers;

/// <summary>
/// Контроллер для авторизации пользователей
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly OutsideContext _outsideContext;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        OutsideContext outsideContext,
        JwtService jwtService,
        ILogger<AuthController> logger)
    {
        _outsideContext = outsideContext;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Вход в систему
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation($"Попытка входа: {loginDto.Login}");

            var user = await _outsideContext.Users
                .FirstOrDefaultAsync(u => u.Login == loginDto.Login);

            if (user == null)
            {
                _logger.LogWarning($"Пользователь не найден: {loginDto.Login}");
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Пользователь заблокирован: {loginDto.Login}");
                return Unauthorized(new { message = "Пользователь заблокирован" });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning($"Неверный пароль для: {loginDto.Login}");
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation($"Успешный вход: {user.Login} ({user.Role})");

            return Ok(new LoginResponseDto
            {
                UserId = user.Id,
                Login = user.Login,
                FullName = user.FullName,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(480)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при входе: {loginDto.Login}");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }

    // ============================================================
    // ПОЛУЧИТЬ ИНФОРМАЦИЮ О ТЕКУЩЕМ ПОЛЬЗОВАТЕЛЕ
    // ============================================================
    /// <summary>
    /// Получить информацию о текущем пользователе
    /// </summary>
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