using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCare360.Enums;

namespace PetCare360.Models;

[Table("alerts")]
public class Alert
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("PET_ID")]
    public long PetId { get; set; }

    [ForeignKey(nameof(PetId))]
    public Pet Pet { get; set; } = null!;

    [Required]
    public AlertTypeEnum Type { get; set; }

    [Required]
    [MaxLength(255)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [Column("LEVEL_ALERT")]
    public AlertLevelEnum Level { get; set; }

    [Required]
    [Column("CREATED_AT")]
    public DateTimeOffset CreatedAt { get; set; }
}