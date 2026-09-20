using AppointmentsManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffPortalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppointmentsManagementSystem.Models.ApplicationUser> _userManager;

        public StaffPortalController(ApplicationDbContext context, UserManager<AppointmentsManagementSystem.Models.ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MySchedule()
        {
            var userId = _userManager.GetUserId(User);

            var staffProfile = await _context.Staff
                .Include(s => s.WorkingHours)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (staffProfile == null)
            {
                return View("NoProfile");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Where(a => a.StaffId == staffProfile.Id)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            ViewBag.StaffProfile = staffProfile;
            return View(appointments);
        }
    }
}