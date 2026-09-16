using Microsoft.AspNetCore.Mvc;

namespace AppointmentsManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
