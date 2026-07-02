using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Outside;

[Table("Icons")]
public class Icon
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("icon")]
    public string IconValue { get; set; } = string.Empty;
}