using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Admin
{
    // Запрос на создание планового задания (для админа)
    public class CreatePlannedTaskRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string TaskType { get; set; } = string.Empty; // Scheme, Kit, Thread
        
        public string? KitSchemeCode { get; set; }   // Код комплекта
        public int? SchemeVariantCode { get; set; }  // Код каунта (для схемы)
        public string? BrandCode { get; set; }       // Код бренда (для Thread)
        public string? ColorCode { get; set; }       // Код цвета (для Thread)
        
        [Required]
        public int Quantity { get; set; } = 1;
        
        public string? AssignedToUserId { get; set; }  // Можно сразу назначить пользователя
    }
}