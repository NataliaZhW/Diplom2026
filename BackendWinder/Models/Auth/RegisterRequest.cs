using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Auth
{
    // Модель запроса на регистрацию нового пользователя
    public class RegisterRequest
    {
        // Email пользователя (обязательное поле)
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        // Пароль пользователя (обязательное поле, минимум 6 символов)
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string Password { get; set; } = string.Empty;

        // Полное имя пользователя (ФИО)
        [Required(ErrorMessage = "Имя обязательно")]
        [MaxLength(200, ErrorMessage = "Имя не может быть длиннее 200 символов")]
        public string FullName { get; set; } = string.Empty;
    }
}