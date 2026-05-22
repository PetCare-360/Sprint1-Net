using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Models;

[Table("users")]
public class AppUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("PASSWORD_HASH")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("CREATED_AT")]
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}