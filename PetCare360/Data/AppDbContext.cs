using Microsoft.EntityFrameworkCore;
using PetCare360.Models;

namespace PetCare360.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<SensorData> SensorData => Set<SensorData>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Pet>(e =>
        {
            e.HasIndex(p => p.DeviceId).IsUnique();
            e.HasOne(p => p.User)
             .WithMany(u => u.Pets)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Device>(e =>
        {
            e.HasIndex(d => d.DeviceId).IsUnique();
            e.HasIndex(d => d.PetId).IsUnique();
            e.HasOne(d => d.Pet)
             .WithOne(p => p.Device)
             .HasForeignKey<Device>(d => d.PetId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<SensorData>(e =>
        {
            e.HasOne(s => s.Device)
             .WithMany(d => d.SensorData)
             .HasForeignKey(s => s.DeviceFkId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Alert>(e =>
        {
            e.HasOne(a => a.Pet)
             .WithMany(p => p.Alerts)
             .HasForeignKey(a => a.PetId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(a => a.Type).HasConversion<string>().HasMaxLength(40);
            e.Property(a => a.Level).HasConversion<string>().HasMaxLength(20);
        });
    }
}