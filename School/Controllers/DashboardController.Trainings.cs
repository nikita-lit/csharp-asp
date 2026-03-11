using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.Models;

namespace School.Controllers
{
    public partial class DashboardController : Controller
    {
        public async Task<IActionResult> Trainings()
        {
            var trainings = await _context.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .ToListAsync();
                
            return View("Trainings/Index", trainings);
        }

        public IActionResult CreateTraining()
        {
            ViewData["CourseList"] = new SelectList(_context.Courses.OrderBy(c => c.Name), "Id", "Name");
            ViewData["TeacherList"] = new SelectList(_context.Teachers.OrderBy(t => t.Name), "Id", "Name");
            return View("Trainings/Create");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTraining(Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                SetStatusMessage("record_done", "success");

                return RedirectToAction(nameof(Trainings));
            }

            ViewData["CourseList"] = new SelectList(_context.Courses.OrderBy(c => c.Name), "Id", "Name", training.CourseId);
            ViewData["TeacherList"] = new SelectList(_context.Teachers.OrderBy(t => t.Name), "Id", "Name", training.TeacherId);
            SetStatusMessage("invalid_data", "danger");

            return View("Trainings/Create", training);
        }
    }
}