using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using School.Models;

namespace School.Controllers
{
    public partial class DashboardController : Controller
    {
        public async Task<IActionResult> Teachers()
        {
            return View("Teachers/Index", await _context.Teachers.Include(x => x.IdentityUser).ToListAsync());
        }

        public IActionResult CreateTeacher()
        {
            ViewData["IdentityUserId"] = GetTeachersUsersList();
            return View("Teachers/Create");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(teacher.IdentityUserId);

                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Student");
                    await _userManager.AddToRoleAsync(user, "Teacher");
                }

                _context.Add(teacher);
                await _context.SaveChangesAsync();
                SetStatusMessage("record_done", "success");
                
                return RedirectToAction(nameof(Teachers));
            }

            SetStatusMessage("invalid_data", "danger");
            ViewData["IdentityUserId"] = GetTeachersUsersList(teacher.IdentityUserId);

            return View("Teachers/Create", teacher);
        }

        public SelectList GetTeachersUsersList(string? selectedId = null)
        {
            var excludedRoleIds = _context.Roles
                    .Where(r => r.Name == "Teacher" || r.Name == "Admin")
                    .Select(r => r.Id)
                    .ToList();

            var availableUsers = _context.Users
                .Where(u => !_context.UserRoles.Any(ur => ur.UserId == u.Id && excludedRoleIds.Contains(ur.RoleId)))
                .ToList();

            return new SelectList(availableUsers, "Id", "UserName", selectedId);
        }
    }
}