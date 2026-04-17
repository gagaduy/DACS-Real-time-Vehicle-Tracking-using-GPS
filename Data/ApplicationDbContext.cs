using Microsoft.EntityFrameworkCore;
using DACS.Models;

namespace DACS.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<GPSHistory> GPSHistories { get; set; }
    public DbSet<Geofence> Geofences { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }
    public DbSet<Contract> Contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Account - Role relationship
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Role)
            .WithMany(r => r.Accounts)
            .HasForeignKey(a => a.RoleId);

        // Vehicle - Device (One-to-One)
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Device)
            .WithOne(d => d.Vehicle)
            .HasForeignKey<Device>(d => d.VehicleId);

        // Device - GPSHistory relationship
        modelBuilder.Entity<GPSHistory>(entity =>
        {
            entity.HasOne(g => g.Device)
                .WithMany(d => d.GPSHistories)
                .HasForeignKey(g => g.DeviceId);

            // Thêm index để tối ưu truy vấn theo xe và thời gian
            entity.HasIndex(g => g.Timestamp);
            entity.HasIndex(g => new { g.DeviceId, g.Timestamp });
        });

        // Vehicle - Alert relationship
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Vehicle)
            .WithMany(v => v.Alerts)
            .HasForeignKey(a => a.VehicleId);

        // Account - UserSetting (One-to-One)
        modelBuilder.Entity<Account>()
            .HasOne(a => a.UserSetting)
            .WithOne(u => u.Account)
            .HasForeignKey<UserSetting>(u => u.AccountId);
            
        // Configure Spatial Data for SQL Server
        // Note: UseNetTopologySuite() is called in Program.cs during service registration.
    }
}
