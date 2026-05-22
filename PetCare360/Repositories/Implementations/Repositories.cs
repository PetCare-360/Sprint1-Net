using Microsoft.EntityFrameworkCore;
using PetCare360.Data;
using PetCare360.Enums;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;

namespace PetCare360.Repositories.Implementations;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<AppUser?> GetByEmailAsync(string email) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<AppUser?> GetByIdAsync(long id) =>
        db.Users.FindAsync(id).AsTask();

    public async Task<bool> ExistsByEmailAsync(string email) =>
    await db.Users.CountAsync(u => u.Email == email) > 0;

    public async Task<AppUser> SaveAsync(AppUser user)
    {
        if (user.Id == 0)
        {
            user.CreatedAt = DateTimeOffset.UtcNow;
            db.Users.Add(user);
        }
        else
            db.Users.Update(user);

        await db.SaveChangesAsync();
        return user;
    }
}

public class PetRepository(AppDbContext db) : IPetRepository
{
    public Task<IEnumerable<Pet>> FindByUserEmailAsync(string email) =>
        Task.FromResult<IEnumerable<Pet>>(
            db.Pets.Include(p => p.Device)
                   .Where(p => p.User.Email == email)
                   .OrderBy(p => p.Name)
                   .ToList());

    public async Task<(IEnumerable<Pet> Items, long Total)> FindByUserEmailPagedAsync(string email, int page, int size)
    {
        var query = db.Pets.Include(p => p.Device)
                           .Where(p => p.User.Email == email)
                           .OrderBy(p => p.Name);
        var total = await query.LongCountAsync();
        var items = await query.Skip(page * size).Take(size).ToListAsync();
        return (items, total);
    }

    public Task<Pet?> FindByIdAndUserEmailAsync(long id, string email) =>
        db.Pets.Include(p => p.Device)
               .FirstOrDefaultAsync(p => p.Id == id && p.User.Email == email);

    public async Task<bool> ExistsByDeviceIdAsync(string deviceId) =>
    await db.Pets.CountAsync(p => p.DeviceId == deviceId) > 0;

    public async Task<Pet> SaveAsync(Pet pet)
    {
        if (pet.Id == 0)
        {
            pet.CreatedAt = DateTimeOffset.UtcNow;
            db.Pets.Add(pet);
        }
        else
            db.Pets.Update(pet);

        await db.SaveChangesAsync();
        return pet;
    }

    public async Task DeleteAsync(Pet pet)
    {
        db.Pets.Remove(pet);
        await db.SaveChangesAsync();
    }
}

public class DeviceRepository(AppDbContext db) : IDeviceRepository
{
    public Task<Device?> FindByDeviceIdAsync(string deviceId) =>
        db.Devices.Include(d => d.Pet)
                  .FirstOrDefaultAsync(d => d.DeviceId == deviceId);

    public async Task<bool> ExistsByDeviceIdAsync(string deviceId) =>
    await db.Devices.CountAsync(d => d.DeviceId == deviceId) > 0;

    public async Task<Device> SaveAsync(Device device)
    {
        if (device.Id == 0)
            db.Devices.Add(device);
        else
            db.Devices.Update(device);

        await db.SaveChangesAsync();
        return device;
    }
}

public class SensorDataRepository(AppDbContext db) : ISensorDataRepository
{
    public Task<SensorData?> FindFirstByDevicePetIdOrderByTimestampDescAsync(long petId) =>
        db.SensorData.Where(s => s.Device.PetId == petId)
                     .OrderByDescending(s => s.Timestamp)
                     .FirstOrDefaultAsync();

    public async Task<(IEnumerable<SensorData> Items, long Total)> FindByDevicePetIdPagedAsync(long petId, int page, int size)
    {
        var query = db.SensorData.Where(s => s.Device.PetId == petId)
                                 .OrderByDescending(s => s.Timestamp);
        var total = await query.LongCountAsync();
        var items = await query.Skip(page * size).Take(size).ToListAsync();
        return (items, total);
    }

    public async Task<(IEnumerable<SensorData> Items, long Total)> FindByDevicePetIdAndStatusPagedAsync(long petId, MonitoringStatusEnum status, int page, int size)
    {
        var query = db.SensorData.Where(s => s.Device.PetId == petId && s.Status == status)
                                 .OrderByDescending(s => s.Timestamp);
        var total = await query.LongCountAsync();
        var items = await query.Skip(page * size).Take(size).ToListAsync();
        return (items, total);
    }

    public Task<long> CountByDevicePetIdAsync(long petId) =>
        db.SensorData.LongCountAsync(s => s.Device.PetId == petId);

    public async Task<IEnumerable<SensorData>> FindByDevicePetIdAndTimestampAfterAsync(long petId, DateTimeOffset after) =>
        await db.SensorData.Where(s => s.Device.PetId == petId && s.Timestamp > after).ToListAsync();

    public async Task<SensorData> SaveAsync(SensorData data)
    {
        db.SensorData.Add(data);
        await db.SaveChangesAsync();
        return data;
    }
}

public class AlertRepository(AppDbContext db) : IAlertRepository
{
    public async Task<(IEnumerable<Alert> Items, long Total)> FindByPetIdPagedAsync(long petId, int page, int size)
    {
        var query = db.Alerts.Where(a => a.PetId == petId).OrderByDescending(a => a.CreatedAt);
        var total = await query.LongCountAsync();
        var items = await query.Skip(page * size).Take(size).ToListAsync();
        return (items, total);
    }

    public async Task<(IEnumerable<Alert> Items, long Total)> FindByPetIdAndLevelPagedAsync(long petId, AlertLevelEnum level, int page, int size)
    {
        var query = db.Alerts.Where(a => a.PetId == petId && a.Level == level).OrderByDescending(a => a.CreatedAt);
        var total = await query.LongCountAsync();
        var items = await query.Skip(page * size).Take(size).ToListAsync();
        return (items, total);
    }

    public Task<long> CountByPetIdAsync(long petId) =>
        db.Alerts.LongCountAsync(a => a.PetId == petId);

    public async Task<IEnumerable<Alert>> SaveAllAsync(IEnumerable<Alert> alerts)
    {
        var list = alerts.ToList();
        foreach (var alert in list)
            alert.CreatedAt = DateTimeOffset.UtcNow;
        db.Alerts.AddRange(list);
        await db.SaveChangesAsync();
        return list;
    }
}