using BackendWinder.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWinder.Models.Outside;

[Table("Users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("login")]
    public string Login { get; set; } = string.Empty;

    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("role")]
    public string Role { get; set; } = "winder";

    // СВОЙСТВО ДЛЯ ENUM
    [NotMapped]
    public UserRole RoleEnum
    {
        get => Role.ToLower() switch
        {
            "master" => UserRole.Master,
            "admin" => UserRole.Admin,
            _ => UserRole.Winder
        };
        set => Role = value.ToString().ToLower();
    }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}