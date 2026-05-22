using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare360.Models;

[Table("pets")]
public class Pet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("USER_ID")]
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Age { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Weight { get; set; }

    [Required]
    [MaxLength(120)]
    public string Breed { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    [Column("DEVICE_ID")]
    public string DeviceId { get; set; } = string.Empty;

    [Required]
    [Column("CREATED_AT")]
    public DateTimeOffset CreatedAt { get; set; }

    public Device? Device { get; set; }
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}