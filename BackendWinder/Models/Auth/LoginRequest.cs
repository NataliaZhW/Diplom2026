using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Auth
{
    // Модель запроса на вход
    public class LoginRequest
    {
        // Email пользователя (обязательное поле)
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        // Пароль пользователя (обязательное поле)
        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;
    }
}