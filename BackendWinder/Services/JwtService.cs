using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendWinder.Models.Outside;
using Microsoft.IdentityModel.Tokens;

namespace BackendWinder.Services;

/// <summary>
/// Сервис для работы с JWT токенами
/// </summary>
public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Генерация JWT токена для пользователя
    /// </summary>
    public string GenerateToken(User user)
    {
        // Получаем настройки JWT из appsettings.json
        var secret = _configuration["JWT:Secret"] 
            ?? throw new Exception("JWT Secret not configured");
        var issuer = _configuration["JWT:Issuer"] 
            ?? throw new Exception("JWT Issuer not configured");
        var audience = _configuration["JWT:Audience"] 
            ?? throw new Exception("JWT Audience not configured");
        var expiryMinutes = int.Parse(_configuration["JWT:ExpiryMinutes"] ?? "480");

        // Создаем ключ шифрования
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Добавляем данные пользователя в токен (Claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName)
        };

        // Создаем токен
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        // Возвращаем строку токена
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}