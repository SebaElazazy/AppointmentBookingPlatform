using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointment
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff).ThenInclude(s => s.User)
                .Include(a => a.Service)
                .ToListAsync();
            return View(appointments);
        }

        // GET: Appointment/Details/5
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
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Users, "Id", "FullName", appointment.CustomerId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Specialization", appointment.StaffId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointment/Edit/5
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
