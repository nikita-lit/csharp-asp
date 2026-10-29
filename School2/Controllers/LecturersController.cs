using Microsoft.AspNetCore.Mvc;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.Lecturers;

namespace School2.Controllers;

public class LecturersController(
    School2Context context,
    ILecturersServices lecturersServices) : Controller
{
    public IActionResult Index()
    {
        var result = context.Lecturers
            .Select(x => new LecturerViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Qualifications = x.Qualifications
            })
            .Take(20);

        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LecturerViewModel vm = new();

        return View("CreateUpdate", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LecturerViewModel vm)
    {
        if (vm == null)
            return RedirectToAction("Error", "Home");

        if (!ModelState.IsValid)
            return View("CreateUpdate", vm);

        var dto = new LecturerDTO
        {
            Id = vm.Id ?? Guid.Empty,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Qualifications = vm.Qualifications,
            Files = vm.Files
        };

        var result = await lecturersServices.Create(dto);

        if (result == null)
            return View("CreateUpdate", vm);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();

        var lecturerToUpdate = await lecturersServices.DetailsAsync(id);

        if (lecturerToUpdate is null)
            return NotFound();

        var vm = new LecturerViewModel
        {
            Id = lecturerToUpdate.Id,
            FirstName = lecturerToUpdate.FirstName,
            LastName = lecturerToUpdate.LastName,
            Qualifications = lecturerToUpdate.Qualifications,
            CreatedAt = lecturerToUpdate.CreatedAt,
            ModifiedAt = lecturerToUpdate.ModifiedAt
        };

        return View("CreateUpdate", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(LecturerViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("CreateUpdate", vm);

        var dto = new LecturerDTO
        {
            Id = vm.Id ?? Guid.Empty,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Qualifications = vm.Qualifications,
            Files = vm.Files
        };

        var result = await lecturersServices.Update(dto);

        if (result is null)
            return NotFound();

        return RedirectToAction(nameof(Update), new { id = result.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();

        var result = await lecturersServices.DetailsAsync(id);

        if (result is null)
            return NotFound();

        var vm = new LecturerViewModel
        {
            Id = result.Id,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Qualifications = result.Qualifications
        };

        ViewData["ViewType"] = "details";

        return View("DetailsDelete", vm);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();

        var result = await lecturersServices.DetailsAsync(id);

        if (result is null)
            return NotFound();

        var vm = new LecturerViewModel
        {
            Id = result.Id,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Qualifications = result.Qualifications
        };

        ViewData["ViewType"] = "delete";

        return View("DetailsDelete", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var lecturerToDelete = await lecturersServices.Delete(id);

        if (lecturerToDelete is null)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }
}