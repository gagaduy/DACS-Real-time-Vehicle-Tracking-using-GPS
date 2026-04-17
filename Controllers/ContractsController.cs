using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS.Data;
using DACS.Models;
using DACS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace DACS.Controllers;

[Authorize]
public class ContractsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContractsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Contracts
    public async Task<IActionResult> Index()
    {
        var contracts = await _context.Contracts
            .Include(c => c.Customer)
            .Include(c => c.Vehicle)
            .OrderByDescending(c => c.CreatedDate)
            .ToListAsync();
        return View(contracts);
    }

    // GET: Contracts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var contract = await _context.Contracts
            .Include(c => c.Customer)
            .Include(c => c.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (contract == null) return NotFound();

        return View(contract);
    }

    // GET: Contracts/Create
    public async Task<IActionResult> Create()
    {
        await PrepareSelectList();
        var model = new ContractViewModel
        {
            ContractNumber = $"HD-{DateTime.Now:yyyyMMdd}-{new Random().Next(100, 999)}",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };
        return View(model);
    }

    // POST: Contracts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContractViewModel model)
    {
        if (ModelState.IsValid)
        {
            var days = (model.EndDate - model.StartDate).Days;
            if (days <= 0) days = 1;

            var contract = new Contract
            {
                ContractNumber = model.ContractNumber,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                RentalPrice = model.RentalPrice,
                Deposit = model.Deposit,
                TotalAmount = model.RentalPrice * days,
                CustomerId = model.CustomerId,
                VehicleId = model.VehicleId,
                TermsAndConditions = model.TermsAndConditions,
                Status = "Draft"
            };

            _context.Add(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await PrepareSelectList();
        return View(model);
    }

    // GET: Contracts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();

        var model = new ContractViewModel
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            RentalPrice = contract.RentalPrice,
            Deposit = contract.Deposit,
            CustomerId = contract.CustomerId,
            VehicleId = contract.VehicleId,
            TermsAndConditions = contract.TermsAndConditions,
            Status = contract.Status
        };

        await PrepareSelectList();
        return View(model);
    }

    // POST: Contracts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContractViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var contract = await _context.Contracts.FindAsync(id);
                if (contract == null) return NotFound();

                var days = (model.EndDate - model.StartDate).Days;
                if (days <= 0) days = 1;

                contract.ContractNumber = model.ContractNumber;
                contract.StartDate = model.StartDate;
                contract.EndDate = model.EndDate;
                contract.RentalPrice = model.RentalPrice;
                contract.Deposit = model.Deposit;
                contract.TotalAmount = model.RentalPrice * days;
                contract.CustomerId = model.CustomerId;
                contract.VehicleId = model.VehicleId;
                contract.TermsAndConditions = model.TermsAndConditions;
                contract.Status = model.Status ?? contract.Status;

                // Logic: Update Vehicle Status if contract is Active
                if (contract.Status == "Active")
                {
                    var vehicle = await _context.Vehicles.FindAsync(contract.VehicleId);
                    if (vehicle != null)
                    {
                        vehicle.Status = "Rented";
                        _context.Update(vehicle);
                    }
                }
                else if (contract.Status == "Completed" || contract.Status == "Cancelled")
                {
                    var vehicle = await _context.Vehicles.FindAsync(contract.VehicleId);
                    if (vehicle != null && vehicle.Status == "Rented")
                    {
                        vehicle.Status = "Available";
                        _context.Update(vehicle);
                    }
                }

                _context.Update(contract);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(model.Id.Value)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        await PrepareSelectList();
        return View(model);
    }

    // Activate Contract Action (Quick update)
    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract != null && contract.Status == "Draft")
        {
            contract.Status = "Active";
            var vehicle = await _context.Vehicles.FindAsync(contract.VehicleId);
            if (vehicle != null)
            {
                vehicle.Status = "Rented";
                _context.Update(vehicle);
            }
            _context.Update(contract);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PrepareSelectList()
    {
        ViewBag.CustomerList = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name");
        ViewBag.VehicleList = new SelectList(await _context.Vehicles.ToListAsync(), "Id", "Name");
    }

    private bool ContractExists(int id)
    {
        return _context.Contracts.Any(e => e.Id == id);
    }
}
