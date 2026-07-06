using System.ComponentModel.DataAnnotations;

namespace BackendWinder.DTOs;

public class TaskStatusUpdateDto
{
    [Required(ErrorMessage = "Статус обязателен")]
    [RegularExpression(
        "^(new|materials_requested|materials_received|submitted|reported|archived)$",
        ErrorMessage = "Недопустимый статус")]
    public string Status { get; set; } = string.Empty;
}