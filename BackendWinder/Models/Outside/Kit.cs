using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Outside;

[Table("Kits")]
public class Kit
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("internal_code")]
    public string InternalCode { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("note")]
    public string? Note { get; set; }

    // Навигационное свойство (состав)
    public ICollection<KitComposition> Compositions { get; set; } = new List<KitComposition>();
}