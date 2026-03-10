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
                TempData["Message"] = "training_not_found";
                return RedirectToAction("Index");
            }

            var already = await _context.Registrations.AnyAsync(r => r.TrainingID == trainingId && r.StudentUserID == userId);
            if (already)
            {
                TempData["Message"] = "already_requested";
                return RedirectToAction("Index");
            }

            var reg = new Registration
            {
                TrainingID = trainingId,
                StudentUserID = userId,
                Staatus = "Pending"
            };

            _context.Registrations.Add(reg);
            await _context.SaveChangesAsync();

            TempData["Message"] = "request_sent";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> PendingRegistrations()
        {
            var pending = await _context.Registrations
                .Where(r => r.Staatus == "Pending")
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
            var reg = await _context.Registrations.Include(r => r.Training).FirstOrDefaultAsync(r => r.ID == id);
            if (reg == null)
            {
                TempData["Message"] = "training_not_found";
                return RedirectToAction("PendingRegistrations");
            }

            var approvedCount = await _context.Registrations.CountAsync(r => r.TrainingID == reg.TrainingID && r.Staatus == "Approved");
            if (approvedCount >= reg.Training.MaxOsalejaid)
            {
                TempData["Message"] = "cannot_approve_full";
                return RedirectToAction("PendingRegistrations");
            }

            reg.Staatus = "Approved";
            await _context.SaveChangesAsync();
            TempData["Message"] = "approve_success";
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
                TempData["Message"] = "training_not_found";
                return RedirectToAction("PendingRegistrations");
            }

            reg.Staatus = "Rejected";
            await _context.SaveChangesAsync();
            TempData["Message"] = "reject_success";
            return RedirectToAction("PendingRegistrations");
        }
    }
}