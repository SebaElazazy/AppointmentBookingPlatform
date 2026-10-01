using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var staffList = _context.Staff
                .Include(s => s.User)
                .Select(s => new {
                    Id = s.Id,
                    DisplayText = s.User != null ? $"{s.User.FullName} ({s.Specialization})" : s.Specialization
                })
                .ToList();

            ViewData["StaffId"] = new SelectList(staffList, "Id", "DisplayText");
            return View();
        }

        // POST: AvailabilityException/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("StaffId,Date,IsDayOff,StartTime,EndTime")] AvailabilityException exception)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exception);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var staffList = _context.Staff
                .Include(s => s.User)
                .Select(s => new {
                    Id = s.Id,
                    DisplayText = s.User != null ? $"{s.User.FullName} ({s.Specialization})" : s.Specialization
                })
                .ToList();

            ViewData["StaffId"] = new SelectList(staffList, "Id", "DisplayText", exception.StaffId);
            return View(exception);
        }

        // GET: AvailabilityException/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exception = await _context.AvailabilityExceptions.FindAsync(id);
            if (exception == null) return NotFound();

            var staffList = _context.Staff
                .Include(s => s.User)
                .Select(s => new {
                    Id = s.Id,
                    DisplayText = s.User != null ? $"{s.User.FullName} ({s.Specialization})" : s.Specialization
                })
                .ToList();

            ViewData["StaffId"] = new SelectList(staffList, "Id", "DisplayText", exception.StaffId);
            return View(exception);
        }

        // POST: AvailabilityException/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

            var staffList = _context.Staff
                .Include(s => s.User)
                .Select(s => new {
                    Id = s.Id,
                    DisplayText = s.User != null ? $"{s.User.FullName} ({s.Specialization})" : s.Specialization
                })
                .ToList();

            ViewData["StaffId"] = new SelectList(staffList, "Id", "DisplayText", exception.StaffId);
            return View(exception);
        }

        // GET: AvailabilityException/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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