using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS.Data;
using DACS.Models;
using DACS.Models.DTOs;
using NetTopologySuite.Geometries;
using Microsoft.AspNetCore.SignalR;
using DACS.Hubs;
using DACS.Models.ViewModels;

namespace DACS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GpsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly GeometryFactory _geometryFactory;
    private readonly IHubContext<DashboardHub> _hubContext;

    public GpsController(ApplicationDbContext context, IHubContext<DashboardHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
        // WGS84 SRID is 4326
        _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
    }

    [HttpPost("Update")]
    public async Task<IActionResult> PostGpsData([FromBody] GpsDataDto data)
    {
        var vehicle = await _context.Vehicles.Include(v => v.Device).FirstOrDefaultAsync(v => v.Id == data.VehicleID);
        if (vehicle == null)
        {
            return NotFound("Vehicle not found.");
        }

        if (vehicle.Device == null)
        {
            return BadRequest("Vehicle has no associated device.");
        }

        // 1. Create Point (NTS coordinates are X=Longitude, Y=Latitude)
        var location = _geometryFactory.CreatePoint(new Coordinate(data.Longitude, data.Latitude));

        // 2. Save GPSHistory
        var history = new GPSHistory
        {
            DeviceId = vehicle.Device.Id,
            Timestamp = DateTime.UtcNow,
            Location = location,
            Speed = data.Speed
        };
        _context.GPSHistories.Add(history);

        // 3. Update Vehicle
        vehicle.LastLatitude = data.Latitude;
        vehicle.LastLongitude = data.Longitude;
        vehicle.LastUpdated = DateTime.UtcNow;

        // Cập nhật trạng thái thiết bị thành Online
        if (vehicle.Device != null)
        {
            vehicle.Device.Status = "Online";
        }

        // 4. Overspeed Check
        if (data.Speed > 80)
        {
            var alert = new Alert
            {
                VehicleId = vehicle.Id,
                Timestamp = DateTime.UtcNow,
                Message = $"Xe {vehicle.LicensePlate} ({vehicle.Name}) chạy quá tốc độ: {data.Speed:F1} km/h",
                Type = "OverSpeed",
                Location = location,
                IsProcessed = false
            };
            _context.Alerts.Add(alert);

            // Gửi SignalR Alert Notification cho tất cả các trang
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new {
                title = "Cảnh báo quá tốc độ!",
                message = alert.Message,
                type = "warning",
                timestamp = alert.Timestamp.ToString("HH:mm:ss")
            });
        }

        await _context.SaveChangesAsync();

        // 5. Gửi SignalR Update cho Dashboard (Chỉ gửi thông tin xe vừa cập nhật để tối ưu)
        var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
        var stats = new
        {
            totalVehicles = await _context.Vehicles.CountAsync(),
            onlineVehicles = await _context.Vehicles.CountAsync(v => v.LastUpdated > fiveMinutesAgo),
            totalCustomers = await _context.Customers.CountAsync(),
            pendingAlerts = await _context.Alerts.CountAsync(),
            // Thông tin xe vừa cập nhật (Delta update)
            latestUpdate = new
            {
                id = vehicle.Id,
                name = vehicle.Name,
                licensePlate = vehicle.LicensePlate,
                latitude = data.Latitude,
                longitude = data.Longitude,
                status = vehicle.Status
            }
        };
        await _hubContext.Clients.All.SendAsync("UpdateStats", stats);

        return Ok(new { message = "Data received and processed successfully." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLatestLocation(int id)
    {
        var vehicle = await _context.Vehicles
            .Select(v => new 
            {
                v.Id,
                v.LastLatitude,
                v.LastLongitude,
                v.LastUpdated,
                // Get most recent speed from history if available
                LatestSpeed = _context.GPSHistories
                    .Where(g => g.Device.VehicleId == v.Id)
                    .OrderByDescending(g => g.Timestamp)
                    .Select(g => g.Speed)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return Ok(vehicle);
    }
}
