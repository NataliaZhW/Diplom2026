namespace BackendWinder.Models.Auth
{
    // Модель ответа с профилем пользователя
    public class UserProfileResponse
    {
        // ID пользователя (GUID)
        public string UserId { get; set; } = string.Empty;

        // Email пользователя
        public string Email { get; set; } = string.Empty;

        // Полное имя пользователя
        public string FullName { get; set; } = string.Empty;

        // Список ролей пользователя
        public List<string> Roles { get; set; } = new List<string>();
    }
}