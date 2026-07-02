using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Winder;

/// <summary>
/// Модель бренда производителя (из таблицы BrandManufs в db-winder)
/// </summary>
[Table("BrandManufs")]
public class BrandManuf
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;
}