using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DACS.Data;
using DACS.Models;
using DACS.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DACS.Controllers;

[Authorize(Roles = "Admin,FleetManager")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);

        var viewModel = new DashboardViewModel
        {
            TotalVehicles = await _context.Vehicles.CountAsync(),
            OnlineVehicles = await _context.Vehicles.CountAsync(v => v.LastUpdated > fiveMinutesAgo),
            TotalCustomers = await _context.Customers.CountAsync(),
            PendingAlerts = await _context.Alerts.CountAsync(),
            LatestContracts = await _context.Contracts
                .Include(c => c.Vehicle)
                .Include(c => c.Customer)
                .OrderByDescending(c => c.CreatedDate)
                .Take(5)
                .ToListAsync(),
            VehicleLocations = await _context.Vehicles
                .Where(v => v.LastLatitude.HasValue && v.LastLongitude.HasValue && v.LastLatitude != 0 && v.LastLongitude != 0)
                .Select(v => new VehicleLocationDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    LicensePlate = v.LicensePlate,
                    Latitude = v.LastLatitude.Value,
                    Longitude = v.LastLongitude.Value,
                    Status = v.Status
                })
                .ToListAsync()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
