using Microsoft.AspNetCore.Mvc;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.Lecturers;

namespace School2.Controllers;

public class LecturersController(
    School2Context context,
    ILecturersServices lecturersServices ) : Controller
{
    public IActionResult Index()
    {
        var result = context.Lecturers
            .Select( x => new LecturerViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Qualifications = x.Qualifications,
                ImageId = x.Image.Select( image => (Guid?)image.ImageId ).FirstOrDefault()
            } )
            .Take( 20 );

        return View( result );
    }

    [HttpGet]
    public IActionResult Image( Guid id )
    {
        var image = context.Files.FirstOrDefault( x => x.ImageId == id );

        if ( image?.ImageData == null )
            return NotFound();

        // we allow only jpg and png
        string? contentType = Path.GetExtension( image.ImageTitle )?.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => null
        };

        if ( contentType == null )
            return NotFound();

        return File( image.ImageData, contentType );
    }

    [HttpGet]
    public IActionResult Create()
    {
        LecturerViewModel vm = new();

        return View( "CreateUpdate", vm );
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( LecturerViewModel vm )
    {
        if ( vm == null )
            return RedirectToAction( "Error", "Home" );

        if ( !ModelState.IsValid )
            return View( "CreateUpdate", vm );

        var dto = new LecturerDTO
        {
            Id = vm.Id ?? Guid.Empty,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Qualifications = vm.Qualifications,
            File = vm.File
        };

        var result = await lecturersServices.Create( dto );

        if ( result == null )
            return View( "CreateUpdate", vm );

        return RedirectToAction( nameof(Index) );
    }

    [HttpGet]
    public async Task<IActionResult> Update( Guid id )
    {
        if ( id == Guid.Empty )
            return NotFound();

        var lecturerToUpdate = await lecturersServices.DetailsAsync( id );

        if ( lecturerToUpdate is null )
            return NotFound();

        var vm = new LecturerViewModel
        {
            Id = lecturerToUpdate.Id,
            FirstName = lecturerToUpdate.FirstName,
            LastName = lecturerToUpdate.LastName,
            Qualifications = lecturerToUpdate.Qualifications,
            ImageId = lecturerToUpdate.Image.Select( image => (Guid?)image.ImageId ).FirstOrDefault(),
            CreatedAt = lecturerToUpdate.CreatedAt,
            ModifiedAt = lecturerToUpdate.ModifiedAt
        };

        return View( "CreateUpdate", vm );
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update( LecturerViewModel vm )
    {
        if ( !ModelState.IsValid )
            return View( "CreateUpdate", vm );

        var dto = new LecturerDTO
        {
            Id = vm.Id ?? Guid.Empty,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Qualifications = vm.Qualifications,
            File = vm.File
        };

        var result = await lecturersServices.Update( dto );

        if ( result is null )
            return NotFound();

        return RedirectToAction( nameof(Update), new { id = result.Id } );
    }

    [HttpGet]
    public async Task<IActionResult> Details( Guid id )
    {
        if ( id == Guid.Empty )
            return NotFound();

        var result = await lecturersServices.DetailsAsync( id );

        if ( result is null )
            return NotFound();

        var vm = new LecturerViewModel
        {
            Id = result.Id,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Qualifications = result.Qualifications,
            ImageId = result.Image.Select( image => (Guid?)image.ImageId ).FirstOrDefault()
        };

        ViewData["ViewType"] = "details";

        return View( "DetailsDelete", vm );
    }

    [HttpGet]
    public async Task<IActionResult> Delete( Guid id )
    {
        if ( id == Guid.Empty )
            return NotFound();

        var result = await lecturersServices.DetailsAsync( id );

        if ( result is null )
            return NotFound();

        var vm = new LecturerViewModel
        {
            Id = result.Id,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Qualifications = result.Qualifications,
            ImageId = result.Image.Select( image => (Guid?)image.ImageId ).FirstOrDefault()
        };

        ViewData["ViewType"] = "delete";

        return View( "DetailsDelete", vm );
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed( Guid id )
    {
        var lecturerToDelete = await lecturersServices.Delete( id );

        if ( lecturerToDelete is null )
            return NotFound();

        return RedirectToAction( nameof(Index) );
    }
}