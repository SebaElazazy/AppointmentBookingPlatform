using Microsoft.AspNetCore.Mvc;

namespace AppointmentsManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
