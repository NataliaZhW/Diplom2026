using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Winder;

[Table("Tasks")]
public class WinderTask
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("status")]
    public string Status { get; set; } = "new";

    [Column("item_type")]
    public string ItemType { get; set; } = string.Empty;

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [Column("item_name")]
    public string ItemName { get; set; } = string.Empty;

    [Column("brand_label")]
    public string? BrandLabel { get; set; }

    [Column("count_value")]
    public int? CountValue { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; } = 1;

    [Column("winder_id")]
    public int WinderId { get; set; }

    [Column("assigned_by")]
    public int? AssignedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("assigned_at")]
    public DateTime? AssignedAt { get; set; }

    [Column("materials_requested_at")]
    public DateTime? MaterialsRequestedAt { get; set; }

    [Column("materials_issued_at")]
    public DateTime? MaterialsIssuedAt { get; set; }

    [Column("submitted_at")]
    public DateTime? SubmittedAt { get; set; }

    [Column("accepted_at")]
    public DateTime? AcceptedAt { get; set; }

    [Column("reported_at")]
    public DateTime? ReportedAt { get; set; }

    [Column("archived_at")]
    public DateTime? ArchivedAt { get; set; }

    [Column("note")]
    public string? Note { get; set; }
}