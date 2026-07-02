using BackendWinder.Data;
using BackendWinder.DTOs;
using BackendWinder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    /// <param name="loginDto">Логин и пароль</param>
    /// <returns>JWT токен и данные пользователя</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation($"Попытка входа: {loginDto.Login}");

            // Ищем пользователя по логину
            var user = await _outsideContext.Users
                .FirstOrDefaultAsync(u => u.Login == loginDto.Login);

            // Если пользователь не найден
            if (user == null)
            {
                _logger.LogWarning($"Пользователь не найден: {loginDto.Login}");
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            // Проверяем, активен ли пользователь
            if (!user.IsActive)
            {
                _logger.LogWarning($"Пользователь заблокирован: {loginDto.Login}");
                return Unauthorized(new { message = "Пользователь заблокирован" });
            }

            // Проверяем пароль (используем BCrypt)
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning($"Неверный пароль для: {loginDto.Login}");
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation($"Успешный вход: {user.Login} ({user.Role})");

            // Возвращаем ответ с токеном
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
}