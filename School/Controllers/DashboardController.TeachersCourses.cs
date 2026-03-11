using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using School.Models;

namespace School.Controllers
{
    public partial class DashboardController : Controller
    {
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> TeacherCourses(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            Teacher? teacher = null;
            if (User.IsInRole("Admin") && id.HasValue)
            {
                teacher = await _context.Teachers.FindAsync(id.Value);
            }

            if (teacher == null)
                teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.IdentityUserId == userId);

            if (User.IsInRole("Admin"))
            {
                var allTeachers = await _context.Teachers.OrderBy(t => t.Name).ToListAsync();
                ViewBag.TeacherList = new SelectList(allTeachers, "Id", "Name", id);
            }

            if (teacher == null)
            {
                TempData["StatusMessage"] = "teacher_profile_not_found";
                TempData["StatusType"] = "danger";
                return View();
            }

            var trainings = new List<Training>();
            if (teacher != null)
            {
                trainings = await _context.Trainings
                    .Where(t => t.TeacherId == teacher.Id)
                    .Include(t => t.Course)
                    .ToListAsync();
            }

            var trainingIds = trainings.Select(t => t.Id).ToList();
            var regs = await _context.Registrations
                .Where(r => trainingIds.Contains(r.TrainingId))
                .Include(r => r.StudentUser)
                .ToListAsync();

            var studentsByTraining = regs
                .GroupBy(r => r.TrainingId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.StudentUser).ToList());

            var model = new TeacherCoursesViewModel
            {
                Teacher = teacher!,
                Trainings = trainings,
                TrainingStudents = studentsByTraining
            };

            return View(model);
        }
    }
}