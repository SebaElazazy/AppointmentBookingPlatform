using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class AvailabilityExceptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityExceptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AvailabilityException
        public async Task<IActionResult> Index()
        {
            var exceptions = await _context.AvailabilityExceptions
                .Include(a => a.Staff)
                .ThenInclude(s => s.User)
                .ToListAsync();
            return View(exceptions);
        }

        // GET: AvailabilityException/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exception = await _context.AvailabilityExceptions
                .Include(a => a.Staff)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (exception == null) return NotFound();

            return View(exception);
        }

        // GET: AvailabilityException/Create
        public IActionResult Create()
        {
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization");
            return View();
        }

        // POST: AvailabilityException/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,Date,IsDayOff,StartTime,EndTime")] AvailabilityException exception)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exception);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", exception.StaffId);
            return View(exception);
        }

        // GET: AvailabilityException/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exception = await _context.AvailabilityExceptions.FindAsync(id);
            if (exception == null) return NotFound();

            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", exception.StaffId);
            return View(exception);
        }

        // POST: AvailabilityException/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StaffId,Date,IsDayOff,StartTime,EndTime")] AvailabilityException exception)
        {
            if (id != exception.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exception);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExceptionExists(exception.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", exception.StaffId);
            return View(exception);
        }

        // GET: AvailabilityException/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exception = await _context.AvailabilityExceptions
                .Include(a => a.Staff)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (exception == null) return NotFound();

            return View(exception);
        }

        // POST: AvailabilityException/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exception = await _context.AvailabilityExceptions.FindAsync(id);
            if (exception != null)
            {
                _context.AvailabilityExceptions.Remove(exception);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExceptionExists(int id)
        {
            return _context.AvailabilityExceptions.Any(e => e.Id == id);
        }
    }
}