using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BackendWinder.Services
{
    // Сервис для генерации и валидации JWT токенов
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Генерация JWT токена для пользователя
        public string GenerateToken(string userId, string email, List<string> roles)
        {
            // Получаем настройки JWT из appsettings.json
            var jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("JWT Key not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer not configured");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new Exception("JWT Audience not configured");
            var jwtExpiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "1440");

            // Преобразуем ключ в байты
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Список утверждений (claims) - информация о пользователе в токене
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),      // ID пользователя
                new Claim(JwtRegisteredClaimNames.Email, email),    // Email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Уникальный ID токена
                new Claim("userId", userId)                         // Кастомное поле для удобства
            };

            // Добавляем роли как отдельные claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Создаем токен
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiryMinutes),
                signingCredentials: credentials
            );

            // Возвращаем токен в виде строки
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}