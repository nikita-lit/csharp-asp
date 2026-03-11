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

            Teacher? teacher;
            if (User.IsInRole("Admin"))
            {
                if (id.HasValue)
                    teacher = await _context.Teachers.FindAsync(id.Value);
                else
                    teacher = await _context.Teachers.FirstOrDefaultAsync();
            }
            else
            {
                teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.IdentityUserId == userId);
            }

            if (User.IsInRole("Admin"))
            {
                var allTeachers = await _context.Teachers.OrderBy(t => t.Name).ToListAsync();
                ViewBag.TeacherList = new SelectList(allTeachers, "Id", "Name", id);
            }

            if (teacher == null)
            {
                if (!User.IsInRole("Admin"))
                    SetStatusMessage("teacher_profile_not_found", "danger");

                return View();
            }

            var trainings = await _context.Trainings
                .Where(t => t.TeacherId == teacher.Id)
                .Include(t => t.Course)
                .ToListAsync();

            List<Registration> regs;
            if (trainings.Count > 0)
            {
                var trainingIds = trainings.Select(t => t.Id).ToList();
                regs = await _context.Registrations
                    .Where(r => trainingIds.Contains(r.TrainingId))
                    .Include(r => r.StudentUser)
                    .ToListAsync();
            }
            else
                regs = [];
                
            var studentsByTraining = regs
                .GroupBy(r => r.TrainingId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.StudentUser).Where(s => s != null).ToList());

            var model = new TeacherCoursesViewModel
            {
                Teacher = teacher,
                Trainings = trainings,
                TrainingStudents = studentsByTraining
            };

            return View(model);
        }
    }
}