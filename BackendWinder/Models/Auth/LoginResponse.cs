namespace BackendWinder.Models.Auth
{
    // Модель ответа после успешного входа
    public class LoginResponse
    {
        // JWT токен для аутентификации
        public string Token { get; set; } = string.Empty;

        // ID пользователя (GUID)
        public string UserId { get; set; } = string.Empty;

        // Email пользователя
        public string Email { get; set; } = string.Empty;

        // Полное имя пользователя
        public string FullName { get; set; } = string.Empty;

        // Список ролей пользователя (например, "User", "Admin")
        public List<string> Roles { get; set; } = new List<string>();
    }
}