using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace School.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
                return RedirectToAction("IndexUser");

            var courses = await _context.Courses.ToListAsync();

            var now = DateTime.Now;
            var currentTrainings = await _context.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .Where(t => t.StartDate <= now && t.EndDate >= now)
                .ToListAsync();
            
            var trainingIds = currentTrainings.Select(t => t.Id).ToList();
            var regCounts = await _context.Registrations
                .Where(r => trainingIds.Contains(r.TrainingId) && r.Status == "Approved")
                .GroupBy(r => r.TrainingId)
                .Select(g => new { TrainingId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TrainingId, x => x.Count);

            var model = new HomeViewModel
            {
                Courses = courses,
                CurrentTrainings = currentTrainings,
                RegistrationCounts = regCounts
            };

            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> IndexUser()
        {
            var courses = await _context.Courses.ToListAsync();

            var now = DateTime.Now;
            var currentTrainings = await _context.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .Where(t => t.StartDate <= now && t.EndDate >= now)
                .ToListAsync();
            
            var trainingIds = currentTrainings.Select(t => t.CourseId).ToList();
            var regCounts = await _context.Registrations
                .Where(r => trainingIds.Contains(r.TrainingId) && r.Status == "Approved")
                .GroupBy(r => r.TrainingId)
                .Select(g => new { TrainingId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TrainingId, x => x.Count);

            var model = new HomeViewModel
            {
                Courses = courses,
                CurrentTrainings = currentTrainings,
                RegistrationCounts = regCounts
            };

            return View("IndexUser", model);
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> About()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyCourses()
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string? returnUrl)
        {
            if (string.IsNullOrEmpty(culture))
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
