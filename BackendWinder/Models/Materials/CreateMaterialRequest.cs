using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Materials
{
    // Запрос на создание запроса материалов
    public class CreateMaterialRequest
    {
        [Required]
        public List<int> TaskIds { get; set; } = new List<int>();  // Список ID заданий
        
        [Required]
        public string Materials { get; set; } = "{}";  // JSON с материалами
        
        public string? Notes { get; set; }  // Примечание (например "дозапрос")
    }
}