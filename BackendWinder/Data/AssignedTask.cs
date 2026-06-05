using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Tasks
{
    // Модель задания, выданного мотальщику
    public class AssignedTask
    {
        // Первичный ключ
        [Key]
        public int Id { get; set; }

        // Ссылка на плановое задание из ReferenceDb (может быть NULL для ручных заданий)
        public int? PlannedTaskId { get; set; }

        // Кому выдано (GUID пользователя из AspNetUsers)
        [Required]
        [MaxLength(255)]
        public string AssignedToUserId { get; set; } = string.Empty;

        // Тип задания: "Scheme", "Kit", "Thread"
        [Required]
        [MaxLength(20)]
        public string TaskType { get; set; } = string.Empty;

        // Код комплекта (например "0048", "0037")
        [MaxLength(20)]
        public string? KitSchemeCode { get; set; }

        // Название комплекта (копия для истории)
        [MaxLength(200)]
        public string? KitSchemeName { get; set; }

        // Код бренда (например "2"=ПНК, "3"=DMC)
        [MaxLength(20)]
        public string? BrandCode { get; set; }

        // Код цвета (например "100", "202")
        [MaxLength(20)]
        public string? ColorCode { get; set; }

        // Код каунта (283, 322 и т.д.)
        public int? CountCode { get; set; }

        // Количество (штук, метров или пасм)
        [Required]
        public int Quantity { get; set; } = 1;

        // Примечание (например "магазин", "оптовик")
        public string? Notes { get; set; }

        // Текущий статус задания (New, MaterialsRequested, MaterialsReceived, InProgress, Submitted, Reported, Archived)
        [MaxLength(30)]
        public string Status { get; set; } = "New";

        // Дата выдачи задания
        public DateTime AssignedAt { get; set; } = DateTime.Now;

        // Дата запроса материалов
        public DateTime? MaterialsRequestedAt { get; set; }

        // Дата получения материалов
        public DateTime? MaterialsReceivedAt { get; set; }

        // Дата сдачи готовой работы
        public DateTime? SubmittedAt { get; set; }

        // Дата внесения в отчетность
        public DateTime? ReportedAt { get; set; }

        // Дата архивации
        public DateTime? ArchivedAt { get; set; }
    }
}