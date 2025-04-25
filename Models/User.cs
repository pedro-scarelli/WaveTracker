namespace LoginApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), IsUnique = true, Name = "IX_Users_Email")]
[Table("tb_users")]
public class User(string name, string email, string hashedPassword)
{
    [Key]
    [Column("pk_id_user", TypeName = "uuid")]
    public Guid Id { get; set; }

    [Column("st_name", TypeName = "varchar(100)")]
    public string Name { get; set; } = name;

    [EmailAddress]
    [Column("st_email", TypeName = "varchar(100)")]
    public string Email { get; set; } = email;

    [Column("st_hashed_password", TypeName = "varchar(255)")]
    public string HashedPassword { get; set; } = hashedPassword;

    [Column("dt_created_at", TypeName = "timestamptz")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("dt_deleted_at", TypeName = "timestamptz")]
    public DateTime? DeletedAt { get; set; }
}
