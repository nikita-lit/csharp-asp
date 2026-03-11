using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.Models;

namespace School.Controllers
{
    public partial class DashboardController : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Courses()
        {
            return View("Courses/Index", await _context.Courses.ToListAsync());
        }

        public async Task<IActionResult> CreateCourse()
        {
            return View("Courses/Create");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                SetStatusMessage("record_done", "success");
                
                return RedirectToAction(nameof(Courses));
            }

            SetStatusMessage("invalid_data", "danger");
            return View("Courses/Create", course);
        }

        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            return View("Courses/Edit", course);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                SetStatusMessage("record_done", "success");
                return RedirectToAction(nameof(Courses));
            }

            SetStatusMessage("invalid_data", "danger");
            return View("Courses/Edit", course);
        }

        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var trainingsCount = await _context.Trainings.CountAsync(t => t.CourseId == id);
            ViewData["HasTrainings"] = trainingsCount > 0;
            ViewData["TrainingsCount"] = trainingsCount;

            return View("Courses/Delete", course);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var hasTrainings = await _context.Trainings.AnyAsync(t => t.CourseId == id);
            if (hasTrainings)
            {
                SetStatusMessage("has_related_trainings", "danger");
                return RedirectToAction(nameof(Courses));
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            SetStatusMessage("record_deleted", "success");
            return RedirectToAction(nameof(Courses));
        }
    }
}