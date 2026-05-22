using System.ComponentModel.DataAnnotations;
using PetCare360.Enums;

namespace PetCare360.DTOs.Requests;

public record RegisterRequest(
    [Required][MaxLength(120)] string Name,
    [Required][EmailAddress][MaxLength(160)] string Email,
    [Required][MinLength(6)][MaxLength(80)] string Password
);

public record AuthRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record InitialSensorDataRequest(
    [Required] DateTimeOffset Timestamp,
    [Required][Range(30.0, 45.0)] decimal Temperature,
    [Required][Range(40, 200)] int HeartRate,
    [Required][Range(0, 100)] int ActivityLevel,
    [Range(-90.0, 90.0)] decimal? Latitude,
    [Range(-180.0, 180.0)] decimal? Longitude,
    [Range(0, 100)] int? Battery
);

public record PetRequest(
    [Required][MaxLength(120)] string Name,
    [Required][Range(0, 40)] int Age,
    [Required][Range(0.10, 200.00)] decimal Weight,
    [Required][MaxLength(120)] string Breed,
    [Required][MaxLength(80)] string DeviceId,
    [Required] InitialSensorDataRequest InitialSensorData
);

public record IotDataRequest(
    [Required][MaxLength(80)] string DeviceId,
    [Required] DateTimeOffset Timestamp,
    [Required][Range(30.0, 45.0)] decimal Temperature,
    [Required][Range(40, 200)] int HeartRate,
    [Required][Range(0, 100)] int ActivityLevel,
    [Required][Range(-90.0, 90.0)] decimal Latitude,
    [Required][Range(-180.0, 180.0)] decimal Longitude,
    [Required][Range(0, 100)] int Battery
);