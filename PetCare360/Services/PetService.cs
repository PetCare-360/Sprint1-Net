using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Enums;
using PetCare360.Exceptions;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;
using PetCare360.Services.Interfaces;

namespace PetCare360.Services;

public class PetService(
    IPetRepository petRepository,
    IDeviceRepository deviceRepository,
    ISensorDataRepository sensorDataRepository,
    IAlertRepository alertRepository,
    CurrentUserService currentUserService,
    PetMapper mapper) : IPetService
{
    private static readonly decimal TemperatureAlert = 39.0m;
    private static readonly decimal TemperatureCritical = 41.0m;

    public async Task<PetPageResponse> ListAsync(int page, int size)
    {
        var email = currentUserService.Email();
        var (items, total) = await petRepository.FindByUserEmailPagedAsync(email, page, size);
        var responses = await Task.WhenAll(items.Select(async p => await ToResponseWithStatusAsync(p)));
        return new PetPageResponse(responses, page, size, total, (int)Math.Ceiling(total / (double)size));
    }

    public async Task<IEnumerable<PetResponse>> ListAllAsync()
    {
        var email = currentUserService.Email();
        var pets = await petRepository.FindByUserEmailAsync(email);
        return await Task.WhenAll(pets.Select(async p => await ToResponseWithStatusAsync(p)));
    }

    public async Task<PetResponse> FindAsync(long id)
    {
        var pet = await FindOwnedPetAsync(id);
        return await ToResponseWithStatusAsync(pet);
    }

    public async Task<PetResponse> CreateAsync(PetRequest request)
    {
        var user = await currentUserService.UserAsync();
        var deviceId = request.DeviceId.Trim();

        if (await deviceRepository.ExistsByDeviceIdAsync(deviceId))
            throw new ConflictException("Este device já está vinculado a outro pet.");

        var pet = new Pet
        {
            User = user,
            UserId = user.Id,
            Name = request.Name.Trim(),
            Age = request.Age,
            Weight = request.Weight,
            Breed = request.Breed.Trim(),
            DeviceId = deviceId
        };

        var device = new Device { DeviceId = deviceId, Pet = pet, Status = DeviceStatusEnum.Active };
        pet.Device = device;

        var savedPet = await petRepository.SaveAsync(pet);
        await SaveInitialSensorDataAsync(device, savedPet, request.InitialSensorData);

        return await ToResponseWithStatusAsync(savedPet);
    }

    public async Task<PetResponse> UpdateAsync(long id, PetRequest request)
    {
        var pet = await FindOwnedPetAsync(id);
        var deviceId = request.DeviceId.Trim();

        pet.Name = request.Name.Trim();
        pet.Age = request.Age;
        pet.Weight = request.Weight;
        pet.Breed = request.Breed.Trim();
        pet.DeviceId = deviceId;

        Device currentDevice;
        if (pet.Device is null)
        {
            if (await deviceRepository.ExistsByDeviceIdAsync(deviceId))
                throw new ConflictException("Este device já está vinculado a outro pet.");
            currentDevice = new Device { DeviceId = deviceId, Pet = pet, Status = DeviceStatusEnum.Active };
            pet.Device = currentDevice;
        }
        else if (pet.Device.DeviceId != deviceId)
        {
            if (await deviceRepository.ExistsByDeviceIdAsync(deviceId))
                throw new ConflictException("Este device já está vinculado a outro pet.");
            pet.Device.DeviceId = deviceId;
            pet.Device.Status = DeviceStatusEnum.Active;
            currentDevice = pet.Device;
        }
        else
        {
            currentDevice = pet.Device;
        }

        var savedPet = await petRepository.SaveAsync(pet);
        await SaveInitialSensorDataAsync(currentDevice, savedPet, request.InitialSensorData);

        return await ToResponseWithStatusAsync(savedPet);
    }

    public async Task DeleteAsync(long id)
    {
        var pet = await FindOwnedPetAsync(id);
        await petRepository.DeleteAsync(pet);
    }

    public async Task<PetHealthStatusResponse> HealthStatusAsync(long id)
    {
        var pet = await FindOwnedPetAsync(id);
        var latest = await sensorDataRepository.FindFirstByDevicePetIdOrderByTimestampDescAsync(pet.Id);
        var status = CurrentStatus(latest);

        return new PetHealthStatusResponse(
            pet.Id, pet.Name, pet.DeviceId, status,
            StatusMessage(status),
            latest?.Timestamp,
            mapper.ToSensorDataResponse(latest));
    }

    public async Task<IEnumerable<QuickAlertPetResponse>> QuickAlertsAsync()
    {
        var email = currentUserService.Email();
        var pets = await petRepository.FindByUserEmailAsync(email);
        var result = new List<QuickAlertPetResponse>();

        foreach (var pet in pets)
        {
            var latest = await sensorDataRepository.FindFirstByDevicePetIdOrderByTimestampDescAsync(pet.Id);
            var status = CurrentStatus(latest);
            if (status != MonitoringStatusEnum.Normal)
                result.Add(new QuickAlertPetResponse(
                    pet.Id, pet.Name, pet.DeviceId, status,
                    StatusMessage(status),
                    mapper.ToSensorDataResponse(latest)));
        }

        return result;
    }

    public async Task<ActivitySummaryResponse> ActivitySummaryAsync(long id)
    {
        var pet = await FindOwnedPetAsync(id);
        var periodEnd = DateTimeOffset.UtcNow;
        var periodStart = periodEnd.AddHours(-24);
        var readings = (await sensorDataRepository.FindByDevicePetIdAndTimestampAfterAsync(pet.Id, periodStart)).ToList();

        return new ActivitySummaryResponse(
            pet.Id, pet.Name, periodStart, periodEnd,
            readings.Count,
            Average(readings.Select(r => r.Temperature)),
            Average(readings.Select(r => (decimal)r.HeartRate)),
            Average(readings.Select(r => (decimal)r.ActivityLevel)));
    }

    // ---- Helpers públicos ----

    public async Task<Pet> FindOwnedPetAsync(long id)
    {
        var email = currentUserService.Email();
        return await petRepository.FindByIdAndUserEmailAsync(id, email)
            ?? throw new NotFoundException("Pet não encontrado.");
    }

    private async Task<PetResponse> ToResponseWithStatusAsync(Pet pet)
    {
        var latest = await sensorDataRepository.FindFirstByDevicePetIdOrderByTimestampDescAsync(pet.Id);
        return mapper.ToPetResponse(pet, CurrentStatus(latest));
    }

    private async Task SaveInitialSensorDataAsync(Device device, Pet pet, InitialSensorDataRequest request)
    {
        var status = CalculateStatus(request.Temperature, request.HeartRate, request.ActivityLevel, request.Battery);

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
        device.UpdateTelemetry(request.Battery ?? 100, request.Timestamp);
        await deviceRepository.SaveAsync(device);
        await CreateAlertsAsync(pet, request.Temperature, request.HeartRate, request.ActivityLevel, request.Battery);
    }

    public static MonitoringStatusEnum CalculateStatus(decimal temperature, int heartRate, int activityLevel, int? battery)
    {
        if (temperature >= TemperatureCritical || heartRate >= 160)
            return MonitoringStatusEnum.Critico;

        if (temperature > TemperatureAlert || heartRate > 130 || activityLevel < 15 || (battery.HasValue && battery < 20))
            return MonitoringStatusEnum.Alerta;

        return MonitoringStatusEnum.Normal;
    }

    private async Task CreateAlertsAsync(Pet pet, decimal temperature, int heartRate, int activityLevel, int? battery)
    {
        var alerts = new List<Alert>();

        if (temperature >= TemperatureCritical)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighTemperature, AlertLevelEnum.Critico, $"Temperatura crítica detectada: {temperature} °C"));
        else if (temperature > TemperatureAlert)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighTemperature, AlertLevelEnum.Alerta, $"Temperatura acima do ideal: {temperature} °C"));

        if (heartRate >= 160)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighHeartRate, AlertLevelEnum.Critico, $"Batimentos cardíacos em nível crítico: {heartRate} bpm"));
        else if (heartRate > 130)
            alerts.Add(NewAlert(pet, AlertTypeEnum.HighHeartRate, AlertLevelEnum.Alerta, $"Batimentos cardíacos elevados: {heartRate} bpm"));

        if (activityLevel < 15)
            alerts.Add(NewAlert(pet, AlertTypeEnum.LowActivity, AlertLevelEnum.Alerta, $"Nível de atividade muito baixo: {activityLevel}%"));

        if (battery.HasValue && battery < 20)
            alerts.Add(NewAlert(pet, AlertTypeEnum.LowBattery, AlertLevelEnum.Alerta, $"Bateria baixa da coleira: {battery}%"));

        if (alerts.Count > 0)
            await alertRepository.SaveAllAsync(alerts);
    }

    private static Alert NewAlert(Pet pet, AlertTypeEnum type, AlertLevelEnum level, string message) =>
        new() { Pet = pet, PetId = pet.Id, Type = type, Level = level, Message = message };

    private static MonitoringStatusEnum CurrentStatus(SensorData? latest) =>
        latest?.Status ?? MonitoringStatusEnum.Normal;

    private static string StatusMessage(MonitoringStatusEnum status) => status switch
    {
        MonitoringStatusEnum.Critico => "Sinais vitais em estado crítico",
        MonitoringStatusEnum.Alerta => "Sinais vitais fora do padrão normal",
        _ => "Tudo bem com o pet"
    };

    private static decimal Average(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        if (list.Count == 0) return 0m;
        return Math.Round(list.Average(), 2);
    }
}