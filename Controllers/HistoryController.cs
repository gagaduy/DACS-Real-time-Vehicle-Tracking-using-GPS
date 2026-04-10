using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS.Data;
using DACS.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS.Controllers;

[Authorize(Roles = "Admin,FleetManager")]
public class HistoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public HistoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: History
    public async Task<IActionResult> Index()
    {
        // Phục vụ cho Dropdown chọn xe
        ViewBag.Vehicles = await _context.Vehicles.ToListAsync();
        return View();
    }

    // API lấy dữ liệu lịch sử
    [HttpGet]
    public async Task<IActionResult> GetHistoryData(int vehicleId, DateTime startTime, DateTime endTime)
    {
        var history = await _context.GPSHistories
            .Where(g => g.Device.VehicleId == vehicleId && 
                        g.Timestamp >= startTime && 
                        g.Timestamp <= endTime)
            .OrderBy(g => g.Timestamp)
            .Select(g => new
            {
                latitude = g.Location.Y,
                longitude = g.Location.X,
                speed = g.Speed,
                timestamp = g.Timestamp
            })
            .ToListAsync();

        return Ok(history);
    }
}
