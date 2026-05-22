using PetCare360.Enums;

namespace PetCare360.DTOs.Responses;

public record UserResponse(long Id, string Name, string Email);

public record AuthResponse(string Token, string Type, UserResponse User);

public record DeviceResponse(
    string DeviceId,
    DeviceStatusEnum Status,
    int? Battery,
    DateTimeOffset? LastSeen
);

public record PetResponse(
    long Id,
    string Name,
    int Age,
    decimal Weight,
    string Breed,
    string DeviceId,
    MonitoringStatusEnum? CurrentStatus,
    DeviceResponse? Device,
    DateTimeOffset CreatedAt
);

public record PetPageResponse(
    IEnumerable<PetResponse> Pets,
    int Page,
    int Size,
    long TotalElements,
    int TotalPages
);

public record SensorDataResponse(
    long Id,
    DateTimeOffset Timestamp,
    decimal Temperature,
    int HeartRate,
    int ActivityLevel,
    decimal? Latitude,
    decimal? Longitude,
    int? Battery,
    MonitoringStatusEnum Status
);

public record AlertResponse(
    long Id,
    AlertTypeEnum Type,
    string Message,
    AlertLevelEnum Level,
    DateTimeOffset CreatedAt
);

public record PetSummaryResponse(
    PetResponse Pet,
    SensorDataResponse? LatestData,
    MonitoringStatusEnum CurrentStatus,
    long TotalReadings,
    long TotalAlerts
);

public record PetHealthStatusResponse(
    long PetId,
    string Name,
    string DeviceId,
    MonitoringStatusEnum CurrentStatus,
    string Message,
    DateTimeOffset? LastSeen,
    SensorDataResponse? LatestData
);

public record QuickAlertPetResponse(
    long PetId,
    string Name,
    string DeviceId,
    MonitoringStatusEnum CurrentStatus,
    string Reason,
    SensorDataResponse? LatestData
);

public record ActivitySummaryResponse(
    long PetId,
    string Name,
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    int Readings,
    decimal AverageTemperature,
    decimal AverageHeartRate,
    decimal AverageActivityLevel
);

public record IotDataResponse(
    long SensorDataId,
    string DeviceId,
    long PetId,
    MonitoringStatusEnum Status,
    DateTimeOffset ProcessedAt,
    IEnumerable<AlertResponse> Alerts
);

public record PagedResponse<T>(
    IEnumerable<T> Data,
    int Page,
    int Size,
    long TotalElements,
    int TotalPages
);