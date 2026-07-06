namespace BackendWinder.DTOs;

/// <summary>
/// DTO для задания
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string ItemTypeLabel { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? BrandLabel { get; set; }
    public int? CountValue { get; set; }
    public int Quantity { get; set; }
    public int WinderId { get; set; }
    public string WinderName { get; set; } = string.Empty;
    public string? AssignedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? MaterialsIssuedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? Note { get; set; }
}