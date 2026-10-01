using System.Diagnostics;
using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredServices = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .Take(4)
                .ToListAsync();

            ViewBag.TotalStaff = await _context.Staff.CountAsync();
            ViewBag.TotalServices = await _context.Services.CountAsync(s => s.IsActive);
            ViewBag.TotalBookings = await _context.Appointments.CountAsync();

            return View(featuredServices);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}