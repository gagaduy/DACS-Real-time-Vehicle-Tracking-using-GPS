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
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<GPSHistory> GPSHistories { get; set; }
    public DbSet<Geofence> Geofences { get; set; }
    public DbSet<Alert> Alerts { get; set; }

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

        // Customer - Rental relationship
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);

        // Vehicle - Rental relationship
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Vehicle)
            .WithMany(v => v.Rentals)
            .HasForeignKey(r => r.VehicleId);

        // Device - GPSHistory relationship
        modelBuilder.Entity<GPSHistory>()
            .HasOne(g => g.Device)
            .WithMany(d => d.GPSHistories)
            .HasForeignKey(g => g.DeviceId);

        // Vehicle - Alert relationship
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Vehicle)
            .WithMany(v => v.Alerts)
            .HasForeignKey(a => a.VehicleId);
            
        // Configure Spatial Data for SQL Server
        // Note: UseNetTopologySuite() is called in Program.cs during service registration.
    }
}
