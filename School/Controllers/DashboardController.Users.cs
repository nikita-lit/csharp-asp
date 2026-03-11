using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace School.Controllers
{
    public partial class DashboardController : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return View("Users/Index", users);
        }
    }
}