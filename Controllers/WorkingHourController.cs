using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class WorkingHourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkingHourController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkingHour
        public async Task<IActionResult> Index()
        {
            var workingHours = await _context.WorkingHours
                .Include(w => w.Staff)
                .ThenInclude(s => s.User)
                .ToListAsync();
            return View(workingHours);
        }

        // GET: WorkingHour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _context.WorkingHours
                .Include(w => w.Staff)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workingHour == null) return NotFound();

            return View(workingHour);
        }

        // GET: WorkingHour/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["StaffId"] = new SelectList(_context.Staff.Include(s => s.User), "Id", "Specialization");
            return View();
        }

        // POST: WorkingHour/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("StaffId,DayOfWeek,StartTime,EndTime")] WorkingHour workingHour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workingHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", workingHour.StaffId);
            return View(workingHour);
        }

        // GET: WorkingHour/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _context.WorkingHours.FindAsync(id);
            if (workingHour == null) return NotFound();

            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", workingHour.StaffId);
            return View(workingHour);
        }

        // POST: WorkingHour/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StaffId,DayOfWeek,StartTime,EndTime")] WorkingHour workingHour)
        {
            if (id != workingHour.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workingHour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkingHourExists(workingHour.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", workingHour.StaffId);
            return View(workingHour);
        }

        // GET: WorkingHour/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _context.WorkingHours
                .Include(w => w.Staff)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workingHour == null) return NotFound();

            return View(workingHour);
        }

        // POST: WorkingHour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workingHour = await _context.WorkingHours.FindAsync(id);
            if (workingHour != null)
            {
                _context.WorkingHours.Remove(workingHour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkingHourExists(int id)
        {
            return _context.WorkingHours.Any(e => e.Id == id);
        }
    }
}