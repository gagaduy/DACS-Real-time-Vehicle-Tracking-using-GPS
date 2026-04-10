using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DACS.Data;
using DACS.Services;
using DACS.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()));

// Đăng ký Authentication với Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddScoped<IGeofenceService, GeofenceService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Phải nằm trước UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<DACS.Hubs.DashboardHub>("/dashboardHub");
app.MapControllers();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    context.Database.Migrate();

    // 1. Seed Roles
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { Name = "Admin" },
            new Role { Name = "FleetManager" }
        );
        context.SaveChanges();
    }

    // 2. Seed Accounts
    if (!context.Accounts.Any())
    {
        var adminRole = context.Roles.First(r => r.Name == "Admin");
        var managerRole = context.Roles.First(r => r.Name == "FleetManager");

        context.Accounts.AddRange(
            new Account 
            { 
                Username = "admin", 
                Password = "123", // Trong thực tế nên hash mật khẩu
                FullName = "Quản trị viên Duy", 
                RoleId = adminRole.Id 
            },
            new Account 
            { 
                Username = "manager", 
                Password = "123", 
                FullName = "Quản lý Đội xe", 
                RoleId = managerRole.Id 
            }
        );
        context.SaveChanges();
    }

    // 3. Seed Vehicles
    if (!context.Vehicles.Any())
    {
        var vehicle = new Vehicle
        {
            Name = "Toyota Vios Demo",
            LicensePlate = "51H-123.45",
            Model = "Sedan",
            Status = "Available"
        };
        context.Vehicles.Add(vehicle);
        context.SaveChanges();
    }

    // 4. Seed Device
    var vehicle1 = context.Vehicles.FirstOrDefault(v => v.Id == 1);
    if (vehicle1 != null && vehicle1.DeviceId == null)
    {
        var device = new Device
        {
            SerialNumber = "DEV-8888",
            Status = "Online",
            VehicleId = vehicle1.Id
        };
        context.Devices.Add(device);
        context.SaveChanges();
        
        vehicle1.DeviceId = device.Id;
        context.SaveChanges();
    }
}

app.Run();
