using BackendWinder.Data;
using BackendWinder.Models.Outside;
using BackendWinder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BackendWinder.Models.Enums;

namespace BackendWinder.Services;

/// <summary>
/// Сервис авторизации
/// </summary>
public class AuthService : IAuthService
{
    private readonly OutsideContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        OutsideContext context,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<LoginResult> LoginAsync(string login, string password)
    {
        try
        {
            _logger.LogInformation($"Попытка входа: {login}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
            {
                _logger.LogWarning($"Пользователь не найден: {login}");
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Неверный логин или пароль"
                };
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Пользователь заблокирован: {login}");
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Пользователь заблокирован"
                };
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning($"Неверный пароль для: {login}");
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Неверный логин или пароль"
                };
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation($"Успешный вход: {user.Login} ({user.Role})");

            return new LoginResult
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Login = user.Login,
                FullName = user.FullName,
                Role = user.Role
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при входе: {login}");
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Внутренняя ошибка сервера"
            };
        }
    }
}