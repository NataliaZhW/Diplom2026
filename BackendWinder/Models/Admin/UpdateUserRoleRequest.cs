using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Admin
{
    // Запрос на обновление роли пользователя
    public class UpdateUserRoleRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = string.Empty; // "Admin" или "User"
        
        [Required]
        public bool Add { get; set; } = true; // true = добавить роль, false = удалить
    }
}