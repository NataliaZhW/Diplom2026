using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Outside;

/// <summary>
/// Модель цвета ниток (из таблицы ColorThreads в db-outside)
/// </summary>
[Table("ColorThreads")]
public class ColorThread
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("pnk")]
    public string? Pnk { get; set; }

    [Column("dmc")]
    public string? Dmc { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}