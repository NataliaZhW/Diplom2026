using System.ComponentModel.DataAnnotations;

namespace BackendWinder.Models.Tasks
{
    // Запрос на изменение статуса задания
    public class UpdateTaskStatusRequest
    {
        [Required]
        public int TaskId { get; set; }
        
        [Required]
        public string NewStatus { get; set; } = string.Empty;
        // Допустимые статусы: MaterialsRequested, MaterialsReceived, InProgress, Submitted, Reported, Archived
    }
}