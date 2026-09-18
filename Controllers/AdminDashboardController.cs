using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCustomers = await _context.Users.CountAsync();
            ViewBag.TotalStaff = await _context.Staff.CountAsync();
            ViewBag.TotalServices = await _context.Services.CountAsync();

            ViewBag.PendingCount = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending);
            ViewBag.ConfirmedCount = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Confirmed);
            ViewBag.CompletedCount = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Completed);
            ViewBag.CancelledCount = await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Cancelled);

            ViewBag.TotalBookings = await _context.Appointments.CountAsync();

            ViewBag.RecentAppointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff).ThenInclude(s => s.User)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}