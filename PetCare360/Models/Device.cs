using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCare360.Enums;

namespace PetCare360.Models;

[Table("devices")]
public class Device
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [MaxLength(80)]
    [Column("DEVICE_ID")]
    public string DeviceId { get; set; } = string.Empty;

    [Required]
    [Column("PET_ID")]
    public long PetId { get; set; }

    [ForeignKey(nameof(PetId))]
    public Pet Pet { get; set; } = null!;

    [Required]
    public DeviceStatusEnum Status { get; set; } = DeviceStatusEnum.Active;

    public int? Battery { get; set; }

    [Column("LAST_SEEN")]
    public DateTimeOffset? LastSeen { get; set; }

    public ICollection<SensorData> SensorData { get; set; } = new List<SensorData>();

    public bool IsActive() => Status == DeviceStatusEnum.Active;

    public void UpdateTelemetry(int battery, DateTimeOffset lastSeen)
    {
        Battery = battery;
        LastSeen = lastSeen;
    }
}