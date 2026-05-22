using PetCare360.DTOs.Responses;
using PetCare360.Enums;
using PetCare360.Repositories.Interfaces;
using PetCare360.Services.Interfaces;

namespace PetCare360.Services;

public class MonitoringService(
    PetService petService,
    ISensorDataRepository sensorDataRepository,
    IAlertRepository alertRepository,
    PetMapper mapper) : IMonitoringService
{
    public async Task<PetSummaryResponse> SummaryAsync(long petId)
    {
        var pet = await petService.FindOwnedPetAsync(petId);
        var latest = await sensorDataRepository.FindFirstByDevicePetIdOrderByTimestampDescAsync(pet.Id);
        var currentStatus = latest?.Status ?? MonitoringStatusEnum.Normal;
        var totalReadings = await sensorDataRepository.CountByDevicePetIdAsync(pet.Id);
        var totalAlerts = await alertRepository.CountByPetIdAsync(pet.Id);

        return new PetSummaryResponse(
            mapper.ToPetResponse(pet, currentStatus),
            mapper.ToSensorDataResponse(latest),
            currentStatus,
            totalReadings,
            totalAlerts);
    }

    public async Task<PagedResponse<SensorDataResponse>> MonitoringAsync(long petId, int page, int size)
    {
        await petService.FindOwnedPetAsync(petId);
        var (items, total) = await sensorDataRepository.FindByDevicePetIdPagedAsync(petId, page, size);
        return Paged(items.Select(s => mapper.ToSensorDataResponse(s)!), page, size, total);
    }

    public async Task<PagedResponse<SensorDataResponse>> ActivityAsync(long petId, int page, int size)
    {
        await petService.FindOwnedPetAsync(petId);
        var (items, total) = await sensorDataRepository.FindByDevicePetIdPagedAsync(petId, page, size);
        return Paged(items.Select(s => mapper.ToSensorDataResponse(s)!), page, size, total);
    }

    public async Task<SensorDataResponse?> LocationAsync(long petId)
    {
        await petService.FindOwnedPetAsync(petId);
        var latest = await sensorDataRepository.FindFirstByDevicePetIdOrderByTimestampDescAsync(petId);
        return mapper.ToSensorDataResponse(latest);
    }

    public async Task<PagedResponse<AlertResponse>> AlertsAsync(long petId, int page, int size)
    {
        await petService.FindOwnedPetAsync(petId);
        var (items, total) = await alertRepository.FindByPetIdPagedAsync(petId, page, size);
        return Paged(items.Select(mapper.ToAlertResponse), page, size, total);
    }

    private static PagedResponse<T> Paged<T>(IEnumerable<T> data, int page, int size, long total) =>
        new(data, page, size, total, (int)Math.Ceiling(total / (double)size));
}