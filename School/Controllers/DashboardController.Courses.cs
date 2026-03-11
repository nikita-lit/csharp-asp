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
    }
}