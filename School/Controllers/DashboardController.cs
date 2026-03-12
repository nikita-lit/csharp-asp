using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using School.Data;
using School.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using School;
using System.Net;
using System.Net.Mail;

namespace School.Controllers
{
    public partial class DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IStringLocalizer<LanguageResources> localizer, IConfiguration configuration) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IStringLocalizer<LanguageResources> _localizer = localizer;
        private readonly IConfiguration _configuration = configuration;

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

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Notify(string? studentId)
        {
            var model = new NotifyViewModel();

            // Populate students dropdown with users in role 'Student' (value = email)
            var students = await _userManager.GetUsersInRoleAsync("Student");
            model.Students = students
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem { Value = u.Email ?? u.Id, Text = u.Email ?? u.UserName ?? u.Id })
                .ToList();

            model.StudentUserId = studentId;

            return View("Notify", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Notify(NotifyViewModel model)
        {
            if (string.IsNullOrEmpty(model.StudentUserId))
            {
                SetStatusMessage(_localizer["please_select_student"], "danger");
                return RedirectToAction(nameof(Notify));
            }

            var user = await _userManager.FindByEmailAsync(model.StudentUserId);
            if (user == null)
            {
                SetStatusMessage(_localizer["student_not_found"], "danger");
                return RedirectToAction(nameof(Notify));
            }

            var recipients = new List<(string name, string email)>();
            recipients.Add((user.UserName ?? user.Email ?? user.Id, user.Email ?? string.Empty));

            // Try sending via SMTP if configured
            if (!string.IsNullOrEmpty(recipients[0].email))
            {
                try
                {
                    var smtpHost = _configuration["Smtp:Host"];
                    var smtpPort = int.TryParse(_configuration["Smtp:Port"], out var p) ? p : 587;
                    var smtpUser = _configuration["Smtp:User"];
                    var smtpPass = _configuration["Smtp:Pass"];
                    var from = _configuration["Smtp:From"] ?? smtpUser;

                    if (!string.IsNullOrEmpty(smtpHost) && !string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPass))
                    {
                        var to = recipients.Select(r => r.email).First();
                        var message = new MailMessage(from, to)
                        {
                            Subject = model.Subject,
                            Body = model.Body
                        };

                        using var client = new SmtpClient(smtpHost, smtpPort)
                        {
                            EnableSsl = true,
                            Credentials = new NetworkCredential(smtpUser, smtpPass)
                        };

                        await client.SendMailAsync(message);
                        SetStatusMessage(_localizer["message_sent"], "success");
                    }
                    else
                    {
                        SetStatusMessage(_localizer["send_error"], "warning");
                    }
                }
                catch (System.Exception ex)
                {
                    SetStatusMessage(_localizer["send_error"] + ": " + ex.Message, "warning");
                }
            }

            var result = new NotifyResult
            {
                Subject = model.Subject,
                Body = model.Body,
                RecipientEmail = recipients.Select(x => x.name).First(),
            };

            return View("NotifyResult", result);
        }
    }
}