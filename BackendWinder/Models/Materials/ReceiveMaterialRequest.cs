using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Materials
{
    // Запрос на подтверждение получения материалов
    public class ReceiveMaterialRequest
    {
        [Required]
        public int RequestId { get; set; }
    }
}