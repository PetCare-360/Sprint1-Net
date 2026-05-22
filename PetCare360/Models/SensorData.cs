using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetCare360.Enums;

namespace PetCare360.Models;

[Table("sensor_data")]
public class SensorData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("DEVICE_ID_FK")]
    public long DeviceFkId { get; set; }

    [ForeignKey(nameof(DeviceFkId))]
    public Device Device { get; set; } = null!;

    [Required]
    [Column("SENSOR_TIMESTAMP")]
    public DateTimeOffset Timestamp { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Temperature { get; set; }

    [Required]
    [Column("HEART_RATE")]
    public int HeartRate { get; set; }

    [Required]
    [Column("ACTIVITY_LEVEL")]
    public int ActivityLevel { get; set; }

    [Column(TypeName = "decimal(10,6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(10,6)")]
    public decimal? Longitude { get; set; }

    public int? Battery { get; set; }

    [Required]
    public MonitoringStatusEnum Status { get; set; }
}