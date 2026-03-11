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
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int trainingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var training = await _context.Trainings.FindAsync(trainingId);
            if (training == null)
            {
                TempData["StatusMessage"] = "training_not_found";
                TempData["StatusType"] = "danger";
                return RedirectToAction("Index");
            }

            var already = await _context.Registrations.AnyAsync(r => r.TrainingId == trainingId && r.StudentUserId == userId);
            if (already)
            {
                TempData["StatusMessage"] = "already_requested";
                TempData["StatusType"] = "danger";
                return RedirectToAction("Index");
            }

            var reg = new Registration
            {
                TrainingId = trainingId,
                StudentUserId = userId,
                Status = "Pending"
            };

            _context.Registrations.Add(reg);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "request_sent";
            TempData["StatusType"] = "success";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> PendingRegistrations()
        {
            var pending = await _context.Registrations
                .Where(r => r.Status == "Pending")
                .Include(r => r.StudentUser)
                .Include(r => r.Training).ThenInclude(t => t.Course)
                .Include(r => r.Training).ThenInclude(t => t.Teacher)
                .ToListAsync();

            return View(pending);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRegistration(int id)
        {
            var reg = await _context.Registrations.Include(r => r.Training).FirstOrDefaultAsync(r => r.Id == id);
            if (reg == null)
            {
                TempData["StatusMessage"] = "training_not_found";
                TempData["StatusType"] = "danger";
                return RedirectToAction("PendingRegistrations");
            }

            var approvedCount = await _context.Registrations.CountAsync(r => r.TrainingId == reg.TrainingId && r.Status == "Approved");
            if (approvedCount >= reg.Training.MaxParticipants)
            {
                TempData["StatusMessage"] = "cannot_approve_full";
                TempData["StatusType"] = "danger";
                return RedirectToAction("PendingRegistrations");
            }

            reg.Status = "Approved";
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "approve_success";
            TempData["StatusType"] = "success";

            return RedirectToAction("PendingRegistrations");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRegistration(int id)
        {
            var reg = await _context.Registrations.FindAsync(id);
            if (reg == null)
            {
                TempData["StatusMessage"] = "training_not_found";
                TempData["StatusType"] = "danger";
                return RedirectToAction("PendingRegistrations");
            }

            reg.Status = "Rejected";
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "reject_success";
            TempData["StatusType"] = "success";

            return RedirectToAction("PendingRegistrations");
        }
    }
}