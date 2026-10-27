using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PetCare360.Models;

public class MongoTelemetryDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public long SensorDataId { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public long PetId { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal Temperature { get; set; }

    public int HeartRate { get; set; }

    public int ActivityLevel { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int? Battery { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StoredAt { get; set; } = DateTime.UtcNow;
}
