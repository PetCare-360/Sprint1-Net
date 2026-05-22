using PetCare360.Enums;
using PetCare360.Models;

namespace PetCare360.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> GetByIdAsync(long id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<AppUser> SaveAsync(AppUser user);
}

public interface IPetRepository
{
    Task<IEnumerable<Pet>> FindByUserEmailAsync(string email);
    Task<(IEnumerable<Pet> Items, long Total)> FindByUserEmailPagedAsync(string email, int page, int size);
    Task<Pet?> FindByIdAndUserEmailAsync(long id, string email);
    Task<bool> ExistsByDeviceIdAsync(string deviceId);
    Task<Pet> SaveAsync(Pet pet);
    Task DeleteAsync(Pet pet);
}

public interface IDeviceRepository
{
    Task<Device?> FindByDeviceIdAsync(string deviceId);
    Task<bool> ExistsByDeviceIdAsync(string deviceId);
    Task<Device> SaveAsync(Device device);
}

public interface ISensorDataRepository
{
    Task<SensorData?> FindFirstByDevicePetIdOrderByTimestampDescAsync(long petId);
    Task<(IEnumerable<SensorData> Items, long Total)> FindByDevicePetIdPagedAsync(long petId, int page, int size);
    Task<(IEnumerable<SensorData> Items, long Total)> FindByDevicePetIdAndStatusPagedAsync(long petId, MonitoringStatusEnum status, int page, int size);
    Task<long> CountByDevicePetIdAsync(long petId);
    Task<IEnumerable<SensorData>> FindByDevicePetIdAndTimestampAfterAsync(long petId, DateTimeOffset after);
    Task<SensorData> SaveAsync(SensorData data);
}

public interface IAlertRepository
{
    Task<(IEnumerable<Alert> Items, long Total)> FindByPetIdPagedAsync(long petId, int page, int size);
    Task<(IEnumerable<Alert> Items, long Total)> FindByPetIdAndLevelPagedAsync(long petId, AlertLevelEnum level, int page, int size);
    Task<long> CountByPetIdAsync(long petId);
    Task<IEnumerable<Alert>> SaveAllAsync(IEnumerable<Alert> alerts);
}