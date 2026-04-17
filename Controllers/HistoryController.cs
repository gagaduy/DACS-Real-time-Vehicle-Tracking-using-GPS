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
        // Kiểm tra khoảng thời gian (tối đa 7 ngày để đảm bảo hiệu năng)
        if ((endTime - startTime).TotalDays > 7)
        {
            return BadRequest(new { message = "Khoảng thời gian tra cứu không được quá 7 ngày." });
        }

        var rawHistory = await _context.GPSHistories
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

        // Tối ưu hóa: Nếu quá nhiều điểm (ví dụ > 2000), thực hiện downsampling
        const int maxPoints = 2000;
        if (rawHistory.Count <= maxPoints)
        {
            return Ok(rawHistory);
        }

        var result = new List<object>();
        int skipFactor = rawHistory.Count / maxPoints;
        
        for (int i = 0; i < rawHistory.Count; i++)
        {
            // Luôn lấy điểm đầu, điểm cuối và các điểm cách đều nhau
            if (i == 0 || i == rawHistory.Count - 1 || i % skipFactor == 0)
            {
                result.Add(rawHistory[i]);
            }
        }

        return Ok(result);
    }
}
