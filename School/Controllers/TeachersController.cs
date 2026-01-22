using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;

namespace School.Controllers
{
    public class TeachersController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Teachers.Include(x => x.IdentityUser).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["IdentityUserID"] = new SelectList(
                _context.Users,
                "Id",
                "UserName"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdentityUserID"] = new SelectList(
                _context.Users,
                "Id",
                "UserName",
                teacher.IdentityUserID
            );

            return View(teacher);
        }
    }
}
