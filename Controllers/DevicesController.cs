using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS.Data;
using DACS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DACS.Controllers;

[Authorize(Roles = "Admin")]
public class DevicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DevicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Devices
    public async Task<IActionResult> Index()
    {
        var devices = await _context.Devices
            .Include(d => d.Vehicle)
            .ToListAsync();
        
        // Lấy danh sách xe chưa có thiết bị để gán
        ViewBag.AvailableVehicles = await _context.Vehicles
            .Where(v => v.DeviceId == null)
            .ToListAsync();

        return View(devices);
    }

    // GET: Devices/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Devices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,SerialNumber,Status")] Device device)
    {
        if (ModelState.IsValid)
        {
            _context.Add(device);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(device);
    }

    // GET: Devices/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var device = await _context.Devices.FindAsync(id);
        if (device == null) return NotFound();
        
        return View(device);
    }

    // POST: Devices/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,SerialNumber,Status,VehicleId")] Device device)
    {
        if (id != device.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(device);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(device.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(device);
    }

    // POST: Devices/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device != null)
        {
            // Gỡ bỏ liên kết với xe nếu có
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.DeviceId == id);
            if (vehicle != null)
            {
                vehicle.DeviceId = null;
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đã xóa thiết bị thành công." });
        }
        return NotFound();
    }

    // POST: Devices/Assign
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int deviceId, int vehicleId)
    {
        var device = await _context.Devices.FindAsync(deviceId);
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);

        if (device == null || vehicle == null) return NotFound();

        // 1. Gỡ bỏ thiết bị cũ của xe này (nếu có - mặc dù logic SQL là 1-1)
        // 2. Gỡ bỏ xe cũ của thiết bị này
        var previousVehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.DeviceId == deviceId);
        if (previousVehicle != null) previousVehicle.DeviceId = null;

        // 3. Thiết lập liên kết mới
        device.VehicleId = vehicleId;
        vehicle.DeviceId = deviceId;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DeviceExists(int id)
    {
        return _context.Devices.Any(e => e.Id == id);
    }
}
