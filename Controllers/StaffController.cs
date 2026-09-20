using AppointmentsManagementSystem.Data;
using AppointmentsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentsManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .Include(s => s.StaffServices).ThenInclude(ss => ss.Service)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (staff == null) return NotFound();

            return View(staff);
        }

        // GET: Staff/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewBag.Services = _context.Services.ToList();
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("UserId,Specialization")] Staff staff, IFormFile? ImageFile, int[] selectedServices)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var savePath = Path.Combine("wwwroot", "images", "staff", fileName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    staff.ImageUrl = "/images/staff/" + fileName;
                }

                _context.Add(staff);
                await _context.SaveChangesAsync(); // لازم الأول عشان ياخد staff.Id

                // تحويل الحساب لـStaff: شيل Customer وحط Staff
                var user = await _userManager.FindByIdAsync(staff.UserId);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Customer"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Customer");
                    }
                    if (!await _userManager.IsInRoleAsync(user, "Staff"))
                    {
                        await _userManager.AddToRoleAsync(user, "Staff");
                    }
                }

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Specialization")] Staff staff, IFormFile? ImageFile, int[] selectedServices)
        {
            if (id != staff.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                        var savePath = Path.Combine("wwwroot", "images", "staff", fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        staff.ImageUrl = "/images/staff/" + fileName;
                    }
                    else
                    {
                        var existing = await _context.Staff.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                        staff.ImageUrl = existing?.ImageUrl;
                    }

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

                    // تأكيد إن الحساب لسه محتفظ بـRole "Staff" بعد التعديل
                    var user = await _userManager.FindByIdAsync(staff.UserId);
                    if (user != null)
                    {
                        if (await _userManager.IsInRoleAsync(user, "Customer"))
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Customer");
                        }
                        if (!await _userManager.IsInRoleAsync(user, "Staff"))
                        {
                            await _userManager.AddToRoleAsync(user, "Staff");
                        }
                    }
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                // رجّعي الحساب Customer تاني بعد ما اتشال من Staff
                var user = await _userManager.FindByIdAsync(staff.UserId);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Staff"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Staff");
                    }
                    if (!await _userManager.IsInRoleAsync(user, "Customer"))
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }
                }

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