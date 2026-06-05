using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Materials
{
    // Модель запроса материалов (один запрос может относиться к нескольким заданиям)
    public class MaterialRequest
    {
        // Первичный ключ
        [Key]
        public int Id { get; set; }

        // Список ID заданий, для которых запрашиваются материалы (JSON массив)
        [Required]
        public string TaskIds { get; set; } = "[]";  // Пример: "[101, 102, 203]"

        // Кто запросил материалы (GUID пользователя)
        [Required]
        [MaxLength(255)]
        public string RequestedByUserId { get; set; } = string.Empty;

        // Список запрашиваемых материалов в формате JSON
        [Required]
        public string Materials { get; set; } = "{}";

        // Статус запроса: "Pending", "Received", "Cancelled"
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // Дата запроса
        public DateTime RequestedAt { get; set; } = DateTime.Now;

        // Дата получения материалов
        public DateTime? ReceivedAt { get; set; }

        // Примечание (например "дозапрос", "срочно")
        public string? Notes { get; set; }
    }
}