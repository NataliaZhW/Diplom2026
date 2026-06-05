using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Auth
{
    // Модель запроса на смену пароля
    public class ChangePasswordRequest
    {
        // Текущий пароль
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        public string CurrentPassword { get; set; } = string.Empty;

        // Новый пароль
        [Required(ErrorMessage = "Новый пароль обязателен")]
        [MinLength(6, ErrorMessage = "Новый пароль должен быть не менее 6 символов")]
        public string NewPassword { get; set; } = string.Empty;
    }
}