using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoArtClassLib;

public class Admin
{
    [Key] public int AdminId { get; set; }

    [Required] [MaxLength(256)] public string Email { get; set; } = string.Empty;
    [Required] [MaxLength(50)] public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(64)")]
    public string HashedPassword { get; set; } = string.Empty;
}