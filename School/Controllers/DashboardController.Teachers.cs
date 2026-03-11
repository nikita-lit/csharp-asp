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

        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.IdentityUser)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound();

            return View("Teachers/Edit", teacher);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeacher(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                SetStatusMessage("invalid_data", "danger");
                return View("Teachers/Edit", teacher);
            }

            var existing = await _context.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == teacher.Id);
            if (existing == null) return NotFound();

            // Do not allow changing IdentityUserId through edit — preserve original
            teacher.IdentityUserId = existing.IdentityUserId;

            _context.Update(teacher);
            await _context.SaveChangesAsync();
            SetStatusMessage("record_done", "success");
            return RedirectToAction(nameof(Teachers));
        }

        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.IdentityUser)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound();

            var trainingsCount = await _context.Trainings.CountAsync(t => t.TeacherId == id);
            ViewData["HasTrainings"] = trainingsCount > 0;
            ViewData["TrainingsCount"] = trainingsCount;

            return View("Teachers/Delete", teacher);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeacherConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            var hasTrainings = await _context.Trainings.AnyAsync(t => t.TeacherId == id);
            if (hasTrainings)
            {
                SetStatusMessage("has_related_trainings", "danger");
                return RedirectToAction(nameof(Teachers));
            }

            var user = await _userManager.FindByIdAsync(teacher.IdentityUserId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "Teacher");
                await _userManager.AddToRoleAsync(user, "Student");
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            SetStatusMessage("record_deleted", "success");
            return RedirectToAction(nameof(Teachers));
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