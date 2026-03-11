using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using School.Data;

namespace School.Controllers
{
    public partial class DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var regs = await _context.Registrations
                .Where(r => r.StudentUserId == userId)
                .Include(r => r.Training).ThenInclude(t => t.Course)
                .Include(r => r.Training).ThenInclude(t => t.Teacher)
                .ToListAsync();

            return View(regs);
        }

        public void SetStatusMessage(string msg, string type)
        {
            TempData["StatusMessage"] = msg;
            TempData["StatusType"] = type;
        }
    }
}