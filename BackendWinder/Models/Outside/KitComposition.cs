using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Outside;

[Table("KitCompositions")]
public class KitComposition
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("kit_id")]
    public int KitId { get; set; }

    [Column("icon_id")]
    public int IconId { get; set; }

    [Column("color_id")]
    public int ColorId { get; set; }

    [Column("meterage")]
    public decimal? Meterage { get; set; }

    [Column("count_252")]
    public int Count252 { get; set; }

    [Column("count_272")]
    public int Count272 { get; set; }

    [Column("count_283")]
    public int Count283 { get; set; }

    [Column("count_282")]
    public int Count282 { get; set; }

    [Column("count_302")]
    public int Count302 { get; set; }

    [Column("count_322")]
    public int Count322 { get; set; }

    [Column("count_362")]
    public int Count362 { get; set; }

    [Column("count_401")]
    public int Count401 { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    // Навигационные свойства
    [ForeignKey("KitId")]
    public Kit Kit { get; set; } = null!;

    [ForeignKey("IconId")]
    public Icon Icon { get; set; } = null!;

    [ForeignKey("ColorId")]
    public ColorThread Color { get; set; } = null!;
}