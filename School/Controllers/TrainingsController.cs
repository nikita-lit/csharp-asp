using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;

namespace School.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainingsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var trainings = await _context.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .ToListAsync();
                
            return View(trainings);
        }

        public IActionResult Create()
        {
            ViewData["CourseList"] = new SelectList(_context.Courses.OrderBy(c => c.Name), "ID", "Name");
            ViewData["TeacherList"] = new SelectList(_context.Teachers.OrderBy(t => t.Name), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "record_done";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseList"] = new SelectList(_context.Courses.OrderBy(c => c.Name), "ID", "Name", training.CourseID);
            ViewData["TeacherList"] = new SelectList(_context.Teachers.OrderBy(t => t.Name), "ID", "Name", training.TeacherID);
            return View(training);
        }
    }
}
