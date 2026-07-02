namespace BackendWinder.DTOs;

/// <summary>
/// DTO для запроса на вход
/// </summary>
public class LoginDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}