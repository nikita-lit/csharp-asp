using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace School.Controllers
{
    public partial class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var model = await GetHomeViewModel();
            return View(model);
        }

        public async Task<IActionResult> Courses()
        {
            var model = await GetHomeViewModel();
            return View(model);
        }

        public async Task<IActionResult> Trainings()
        {
            var model = await GetHomeViewModel();
            return View(model);
        }

        public async Task<IActionResult> Teachers()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return View(teachers);
        }

        public async Task<IActionResult> About()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            return View();
        }

        public async Task<HomeViewModel> GetHomeViewModel()
        {
            var now = DateTime.Now;

            var coursesTask = _context.Courses.ToListAsync();
            var currentTrainingsTask = _context.Trainings
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .Where(t => t.EndDate >= DateTime.Now)
                .ToListAsync();

            // wait for both to finish
            await Task.WhenAll(coursesTask, currentTrainingsTask);
            
            var courses = await coursesTask;
            var currentTrainings = await currentTrainingsTask;

            var trainingIds = currentTrainings.Select(t => t.Id).ToList();
            var regCounts = await _context.Registrations
                .Where(r => trainingIds.Contains(r.TrainingId) && r.Status == "Approved")
                .GroupBy(r => r.TrainingId)
                .Select(g => new { TrainingId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TrainingId, x => x.Count);

            // If the user is authenticated, load their registration status per training
            IDictionary<int, string> regStatuses = new Dictionary<int, string>();
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var userRegs = await _context.Registrations
                    .Where(r => trainingIds.Contains(r.TrainingId) && r.StudentUserId == userId)
                    .ToListAsync();

                regStatuses = userRegs.ToDictionary(r => r.TrainingId, r => r.Status ?? "");
            }

            var model = new HomeViewModel
            {
                Courses = courses,
                CurrentTrainings = currentTrainings,
                RegistrationCounts = regCounts,
                RegistrationStatuses = regStatuses
            };

            return model;
        }

        public async Task<IActionResult> SetLanguage(string culture, string? returnUrl)
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