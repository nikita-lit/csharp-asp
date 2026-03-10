using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using School.Data;
using School.Models;

namespace School.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeachersController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Teachers.Include(x => x.IdentityUser).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["IdentityUserID"] = GetUsersList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(teacher.IdentityUserID);

                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Student");
                    await _userManager.AddToRoleAsync(user, "Teacher");
                }

                _context.Add(teacher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "record_done";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdentityUserID"] = GetUsersList(teacher.IdentityUserID);

            return View(teacher);
        }

        public SelectList GetUsersList(string? selectedId = null)
        {
            var excludedRoleIds = _context.Roles
                    .Where(r => r.Name == "Admin" || r.Name == "Teacher")
                    .Select(r => r.Id)
                    .ToList();

            var availableUsers = _context.Users
                .Where(u => !_context.UserRoles.Any(ur => ur.UserId == u.Id && excludedRoleIds.Contains(ur.RoleId)))
                .ToList();

            return new SelectList(availableUsers, "Id", "UserName", selectedId);
        }
    }
}
