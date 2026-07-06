using System.ComponentModel.DataAnnotations;

namespace BackendWinder.DTOs;

public class TaskStatusUpdateDto
{
    [Required(ErrorMessage = "Статус обязателен")]
    [RegularExpression(
        "^(new|planned|materials_requested|materials_issued|in_progress|completed|accepted|archived)$",
        ErrorMessage = "Недопустимый статус")]
    public string Status { get; set; } = string.Empty;
}