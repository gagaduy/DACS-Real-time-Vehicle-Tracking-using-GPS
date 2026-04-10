using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS.Data;
using DACS.Models;
using Microsoft.AspNetCore.Authorization;

namespace DACS.Controllers
{
    [Authorize(Roles = "Admin,FleetManager")]
    public class AlertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var alerts = await _context.Alerts
                .Include(a => a.Vehicle)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
            return View(alerts);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsProcessed(int id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            alert.IsProcessed = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
