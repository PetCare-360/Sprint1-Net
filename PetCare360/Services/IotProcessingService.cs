using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Enums;
using PetCare360.Exceptions;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;
using PetCare360.Services.Interfaces;

namespace PetCare360.Services;

public class IotProcessingService(
    IDeviceRepository deviceRepository,
    ISensorDataRepository sensorDataRepository,
    IAlertRepository alertRepository,
    PetMapper mapper) : IIotProcessingService
{
    private static readonly decimal TemperatureAlert = 39.0m;
    private static readonly decimal TemperatureCritical = 41.0m;

    public async Task<IotDataResponse> ProcessAsync(IotDataRequest request)
    {
        var device = await deviceRepository.FindByDeviceIdAsync(request.DeviceId.Trim())
            ?? throw new NotFoundException("Device não encontrado.");

        if (!device.IsActive())
            throw new ConflictException("Device inativo para recebimento de telemetria.");

        var pet = device.Pet ?? throw new ConflictException("Device não está vinculado a um pet.");

        var status = PetService.CalculateStatus(request.Temperature, request.HeartRate, request.ActivityLevel, request.Battery);

        var sensorData = new SensorData
        {
            Device = device,
            DeviceFkId = device.Id,
            Timestamp = request.Timestamp,
            Temperature = request.Temperature,
            HeartRate = request.HeartRate,
            ActivityLevel = request.ActivityLevel,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Battery = request.Battery,
            Status = status
        };

        await sensorDataRepository.SaveAsync(sensorData);
        device.UpdateTelemetry(request.Battery, request.Timestamp);
        await deviceRepository.SaveAsync(device);

        var alerts = await CreateAlertsAsync(pet, request);

        return new IotDataResponse(
            sensorData.Id,
            device.DeviceId,
            pet.Id,
            status,
            DateTimeOffset.UtcNow,
            alerts.Select(mapper.ToAlertResponse));
    }

    private async Task<IEnumerable<Alert>> CreateAlertsAsync(Pet pet, IotDataRequest request)
    {
        var alerts = new List<Alert>();

        if (request.Temperature >= TemperatureCritical)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighTemperature, AlertLevelEnum.Critico, $"Temperatura crítica detectada: {request.Temperature} °C"));
        else if (request.Temperature > TemperatureAlert)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighTemperature, AlertLevelEnum.Alerta, $"Temperatura acima do ideal: {request.Temperature} °C"));

        if (request.HeartRate >= 160)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighHeartRate, AlertLevelEnum.Critico, $"Batimentos cardíacos em nível crítico: {request.HeartRate} bpm"));
        else if (request.HeartRate > 130)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighHeartRate, AlertLevelEnum.Alerta, $"Batimentos cardíacos elevados: {request.HeartRate} bpm"));

        if (request.ActivityLevel < 15)
            alerts.Add(NewAlert(pet, AlertTypeEnum.LowActivity, AlertLevelEnum.Alerta, $"Nível de atividade muito baixo: {request.ActivityLevel}%"));

        if (request.Battery < 20)
            alerts.Add(NewAlert(pet, AlertTypeEnum.LowBattery, AlertLevelEnum.Alerta, $"Bateria baixa da coleira: {request.Battery}%"));

        if (alerts.Count > 0)
            await alertRepository.SaveAllAsync(alerts);

        return alerts;
    }

    private static Alert NewAlert(Pet pet, AlertTypeEnum type, AlertLevelEnum level, string message) =>
        new() { Pet = pet, PetId = pet.Id, Type = type, Level = level, Message = message };
}