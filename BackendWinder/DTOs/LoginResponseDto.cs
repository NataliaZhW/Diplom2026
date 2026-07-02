namespace BackendWinder.DTOs;

/// <summary>
/// DTO для ответа при успешном входе
/// </summary>
public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}