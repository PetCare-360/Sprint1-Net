using PetCare360.DTOs.Responses;
using PetCare360.Enums;
using PetCare360.Models;

namespace PetCare360.Services;

public class PetMapper
{
    public UserResponse ToUserResponse(AppUser user) =>
        new(user.Id, user.Name, user.Email);

    public PetResponse ToPetResponse(Pet pet, MonitoringStatusEnum? currentStatus = null)
    {
        DeviceResponse? deviceResponse = null;
        if (pet.Device is not null)
            deviceResponse = new DeviceResponse(
                pet.Device.DeviceId,
                pet.Device.Status,
                pet.Device.Battery,
                pet.Device.LastSeen);

        return new PetResponse(
            pet.Id, pet.Name, pet.Age, pet.Weight, pet.Breed,
            pet.DeviceId, currentStatus, deviceResponse, pet.CreatedAt);
    }

    public SensorDataResponse? ToSensorDataResponse(SensorData? data)
    {
        if (data is null) return null;
        return new SensorDataResponse(
            data.Id, data.Timestamp, data.Temperature, data.HeartRate,
            data.ActivityLevel, data.Latitude, data.Longitude, data.Battery, data.Status);
    }

    public AlertResponse ToAlertResponse(Alert alert) =>
        new(alert.Id, alert.Type, alert.Message, alert.Level, alert.CreatedAt);
}