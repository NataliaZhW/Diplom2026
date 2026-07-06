using BackendWinder.Models.Outside;

namespace BackendWinder.Services.Interfaces;

/// <summary>
/// Результат входа в систему
/// </summary>
public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public int? UserId { get; set; }
    public string? Login { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Сервис авторизации
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Вход в систему
    /// </summary>
    Task<LoginResult> LoginAsync(string login, string password);
}