using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff
        public async Task<IActionResult> Index()
        {
            var staff = await _context.Staff
                .Include(s => s.User)
                .ToListAsync();
            return View(staff);
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staff
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (staff == null) return NotFound();

            return View(staff);
        }

        // GET: Staff/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewBag.Services=_context.Services.ToList();
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Specialization,ImageUrl")] Staff staff, int[] selectedServices)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();

                if (selectedServices != null)
                {
                    foreach (var serviceId in selectedServices)
                    {
                        _context.StaffServices.Add(new StaffService
                        {
                            StaffId = staff.Id,
                            ServiceId = serviceId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", staff.UserId);
            ViewBag.Services = _context.Services.ToList();
            return View(staff);
        }

        // GET: Staff/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staff
                .Include(s => s.StaffServices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (staff == null) return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", staff.UserId);
            ViewBag.Services = _context.Services.ToList();
            ViewBag.SelectedServices = staff.StaffServices.Select(ss => ss.ServiceId).ToList();
            return View(staff);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Specialization,ImageUrl")] Staff staff, int[] selectedServices)
        {
            if (id != staff.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);

                    var existingLinks = _context.StaffServices.Where(ss => ss.StaffId == id);
                    _context.StaffServices.RemoveRange(existingLinks);

                    if (selectedServices != null)
                    {
                        foreach (var serviceId in selectedServices)
                        {
                            _context.StaffServices.Add(new StaffService
                            {
                                StaffId = id,
                                ServiceId = serviceId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", staff.UserId);
            ViewBag.Services = _context.Services.ToList();
            return View(staff);
        }

        // GET: Staff/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staff
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (staff == null) return NotFound();

            return View(staff);
        }

        // POST: Staff/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.Id == id);
        }
    }
}