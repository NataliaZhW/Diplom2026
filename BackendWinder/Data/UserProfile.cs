using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Data
{
    // Модель профиля пользователя - дополнительная информация
    // Связана с AspNetUsers через поле UserId
    public class UserProfile
    {
        // Первичный ключ
        [Key]
        public int Id { get; set; }

        // Внешний ключ - ссылка на пользователя из Identity
        // Хранит GUID пользователя
        [Required]
        [MaxLength(255)]
        public string UserId { get; set; } = string.Empty;

        // Полное имя пользователя (ФИО)
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        // Дата создания профиля (автоматически устанавливается при создании)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}