using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager <ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointment
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var query = _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff).ThenInclude(s => s.User)
                .Include(a => a.Service)
                .AsQueryable();

            
            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(a => a.CustomerId == userId);
            }

            var appointments = await query.ToListAsync();
            return View(appointments);
        }

        // GET: Appointment/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff).ThenInclude(s => s.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // GET: Appointment/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("CustomerId,StaffId,ServiceId,AppointmentDate,StartTime,EndTime")] Appointment appointment)
        {
            // منع التعارض (Overlap Check)
            bool hasConflict = await _context.Appointments
                .AnyAsync(a => a.StaffId == appointment.StaffId
                    && a.Status != AppointmentStatus.Cancelled
                    && a.AppointmentDate == appointment.AppointmentDate
                    && a.StartTime < appointment.EndTime
                    && appointment.StartTime < a.EndTime);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "This staff member already has an appointment during this time.");
            }

            if (ModelState.IsValid && !hasConflict)
            {
                appointment.Status = AppointmentStatus.Pending;
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your appointment has been booked successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "FullName", appointment.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", appointment.StaffId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointment/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "FullName", appointment.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", appointment.StaffId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,StaffId,ServiceId,AppointmentDate,StartTime,EndTime,Status")] Appointment appointment)
        {
            if (id != appointment.Id) return NotFound();

            // منع التعارض (مستثنية الـAppointment نفسه من المقارنة)
            bool hasConflict = await _context.Appointments
                .AnyAsync(a => a.Id != appointment.Id
                    && a.StaffId == appointment.StaffId
                    && a.Status != AppointmentStatus.Cancelled
                    && a.AppointmentDate == appointment.AppointmentDate
                    && a.StartTime < appointment.EndTime
                    && appointment.StartTime < a.EndTime);

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "This staff member already has an appointment during this time.");
            }

            if (ModelState.IsValid && !hasConflict)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "FullName", appointment.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", appointment.StaffId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointment/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff).ThenInclude(s => s.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
