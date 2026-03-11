using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using School.Models;

namespace School.Controllers
{
    public partial class DashboardController : Controller
    {
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Register(int trainingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var training = await _context.Trainings.FindAsync(trainingId);
            if (training == null)
            {
                SetStatusMessage("training_not_found", "danger");
                return RedirectToAction("Trainings", "Home");
            }

            // Prevent registration when training is already full
            var approvedCount = await _context.Registrations.CountAsync(r => r.TrainingId == trainingId && r.Status == "Approved");
            if (approvedCount >= training.MaxParticipants)
            {
                SetStatusMessage("full_group", "danger");
                return RedirectToAction("Trainings", "Home");
            }

            var already = await _context.Registrations.AnyAsync(r => r.TrainingId == trainingId && r.StudentUserId == userId);
            if (already)
            {
                SetStatusMessage("already_requested", "danger");
                return RedirectToAction("Trainings", "Home");
            }

            var reg = new Registration
            {
                TrainingId = trainingId,
                StudentUserId = userId,
                Status = "Pending"
            };

            _context.Registrations.Add(reg);
            await _context.SaveChangesAsync();
            SetStatusMessage("request_sent", "success");

            return RedirectToAction("Trainings", "Home");
        }

        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> PendingRegistrations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Admin"))
            {
                var pendingAll = await _context.Registrations
                    .Where(r => r.Status == "Pending")
                    .Include(r => r.StudentUser)
                    .Include(r => r.Training).ThenInclude(t => t.Course)
                    .Include(r => r.Training).ThenInclude(t => t.Teacher)
                    .ToListAsync();

                return View(pendingAll);
            }

            // For teachers: only registrations for trainings taught by this teacher
            var pendingForTeacher = await _context.Registrations
                .Where(r => r.Status == "Pending")
                .Include(r => r.StudentUser)
                .Include(r => r.Training).ThenInclude(t => t.Course)
                .Include(r => r.Training).ThenInclude(t => t.Teacher)
                .Where(r => r.Training != null && r.Training.Teacher != null && r.Training.Teacher.IdentityUserId == userId)
                .ToListAsync();

            return View(pendingForTeacher);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> ApproveRegistration(int id)
        {
            var reg = await _context.Registrations
                .Include(r => r.Training).ThenInclude(t => t.Teacher)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (reg == null)
            {
                SetStatusMessage("training_not_found", "danger");
                return RedirectToAction("PendingRegistrations");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin"))
            {
                // Teachers may only approve registrations for their own trainings
                if (reg.Training == null || reg.Training.Teacher == null || reg.Training.Teacher.IdentityUserId != userId)
                {
                    SetStatusMessage("not_allowed", "danger");
                    return RedirectToAction("PendingRegistrations");
                }
            }

            var approvedCount = await _context.Registrations.CountAsync(r => r.TrainingId == reg.TrainingId && r.Status == "Approved");
            if (approvedCount >= reg.Training.MaxParticipants)
            {
                SetStatusMessage("cannot_approve_full", "danger");
                return RedirectToAction("PendingRegistrations");
            }

            reg.Status = "Approved";
            await _context.SaveChangesAsync();
            SetStatusMessage("approve_success", "success");

            return RedirectToAction("PendingRegistrations");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> RejectRegistration(int id)
        {
            var reg = await _context.Registrations.FindAsync(id);
            if (reg == null)
            {
                SetStatusMessage("training_not_found", "danger");
                return RedirectToAction("PendingRegistrations");
            }

            reg.Status = "Rejected";
            await _context.SaveChangesAsync();
            SetStatusMessage("reject_success", "success");

            return RedirectToAction("PendingRegistrations");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            var reg = await _context.Registrations
                .Include(r => r.Training).ThenInclude(t => t.Teacher)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reg == null)
            {
                SetStatusMessage("training_not_found", "danger");
                return RedirectToAction("PendingRegistrations");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin"))
            {
                if (reg.Training == null || reg.Training.Teacher == null || reg.Training.Teacher.IdentityUserId != userId)
                {
                    SetStatusMessage("not_allowed", "danger");
                    return RedirectToAction("PendingRegistrations");
                }
            }

            _context.Registrations.Remove(reg);
            await _context.SaveChangesAsync();
            SetStatusMessage("delete_success", "success");

            return RedirectToAction("PendingRegistrations");
        }
    }
}