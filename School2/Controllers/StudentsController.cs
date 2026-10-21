using Microsoft.AspNetCore.Mvc;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.Students;

namespace School2.Controllers;

public class StudentsController( School2Context context, IStudentsServices studentsServices ) : Controller
{
    public IActionResult Index()
    {
        var result = context.Students
            .Select(x => new StudentViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email
            }).Take(20);
            
        return View(result);
    }
    
    
    [HttpGet]
    public IActionResult Create()
    {
        StudentViewModel vm = new();
        return View("CreateUpdate", vm);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentViewModel vm) 
    {
        if (vm == null)
            return RedirectToAction("Error", "Home");

        if (!ModelState.IsValid)
            return RedirectToAction("Error", "Home");

        var dto = new StudentDTO() 
        {
            Id = vm.Id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            PhoneNumber = vm.PhoneNumber,
            Email = vm.Email
        };

        var result = await studentsServices.Create(dto);

        if (result == null)
            return RedirectToAction("Error", "Home");
        else
            return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        if (id == Guid.Empty)
            return NotFound();
            
        var studentToUpdate = await studentsServices.DetailsAsync(id);
        if (studentToUpdate is null)
            return NotFound();
            
        var vm = new StudentViewModel();
        vm.Id = studentToUpdate.Id;
        vm.FirstName = studentToUpdate.FirstName;
        vm.LastName = studentToUpdate.LastName;
        vm.PhoneNumber = studentToUpdate.PhoneNumber;
        vm.Email = studentToUpdate.Email;
        vm.CreatedAt = studentToUpdate.CreatedAt;
        vm.ModifiedAt = studentToUpdate.ModifiedAt;
            
        return View("CreateUpdate", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(StudentViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("CreateUpdate", vm);

        var dto = new StudentDTO()
        {
            Id = vm.Id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            PhoneNumber = vm.PhoneNumber,
            Email = vm.Email,
            CreatedAt = (DateTime)vm.CreatedAt,
            ModifiedAt = (DateTime)vm.ModifiedAt
        };
            
        var result = await studentsServices.Update(dto);
        var resultId = result.Id;
        if (result is null)
            return RedirectToAction(nameof(Index));
            
        return RedirectToAction(nameof(Update), new { id = resultId });
    }
}