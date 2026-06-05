using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Tasks
{
    // Запрос на создание нового задания
    public class CreateTaskRequest
    {
        [Required]
        public string TaskType { get; set; } = string.Empty;  // Scheme, Kit, Thread
        
        public string? KitSchemeCode { get; set; }   // Код комплекта (для Scheme/Kit)
        public string? BrandCode { get; set; }       // Код бренда (для Thread)
        public string? ColorCode { get; set; }       // Код цвета (для Thread)
        public int? CountCode { get; set; }          // Код каунта (для Scheme)
        
        [Required]
        public int Quantity { get; set; } = 1;
        
        public string? Notes { get; set; }
    }
}