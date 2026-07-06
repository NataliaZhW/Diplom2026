using System.ComponentModel.DataAnnotations;

namespace BackendWinder.DTOs;

public class TaskCreateDto
{
    [Required(ErrorMessage = "Тип обязателен")]
    [RegularExpression("^(kit|scheme|thread)$", ErrorMessage = "Тип должен быть kit, scheme или thread")]
    public string ItemType { get; set; } = string.Empty;

    [Required(ErrorMessage = "ID элемента обязателен")]
    [Range(1, int.MaxValue, ErrorMessage = "ID должен быть больше 0")]
    public int ItemId { get; set; }

    [Required(ErrorMessage = "Код обязателен")]
    [StringLength(20, ErrorMessage = "Код не должен превышать 20 символов")]
    public string ItemCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string ItemName { get; set; } = string.Empty;

    public string? BrandLabel { get; set; }

    public int? CountValue { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
    public int Quantity { get; set; } = 1;

    [Required(ErrorMessage = "ID мотальщика обязателен")]
    [Range(1, int.MaxValue, ErrorMessage = "ID мотальщика должен быть больше 0")]
    public int WinderId { get; set; }

    public string? Note { get; set; }
}