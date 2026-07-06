using BackendWinder.Models.Outside;

namespace BackendWinder.Services.Interfaces;

/// <summary>
/// Сервис для работы с JWT-токенами
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Генерирует JWT-токен для пользователя
    /// </summary>
    string GenerateToken(User user);
}