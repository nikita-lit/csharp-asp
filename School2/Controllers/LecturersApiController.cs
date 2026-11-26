using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.Lecturers;

namespace School2.Controllers;

[ApiController, Route( "api/lecturers" )]
public class LecturersApiController( School2Context context, ILecturersServices lecturersServices ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LecturerViewModel>>> GetAll()
    {
        var lecturers = await context.Lecturers
            .Select( lecturer => new LecturerViewModel
            {
                Id = lecturer.Id,
                FirstName = lecturer.FirstName,
                LastName = lecturer.LastName,
                Qualifications = lecturer.Qualifications,
                ImageId = lecturer.Image.Select( image => (Guid?)image.ImageId ).FirstOrDefault(),
                CreatedAt = lecturer.CreatedAt,
                ModifiedAt = lecturer.ModifiedAt
            } )
            .Take( 20 )
            .ToListAsync();

        return Ok( lecturers );
    }

    [HttpGet( "{id:guid}" )]
    public async Task<ActionResult<LecturerViewModel>> GetById( Guid id )
    {
        var lecturer = await lecturersServices.DetailsAsync( id );
        if ( lecturer is null )
            return NotFound();

        return Ok( ToViewModel( lecturer ) );
    }

    [HttpPost]
    public async Task<ActionResult<LecturerViewModel>> Create( [FromForm] LecturerViewModel vm )
    {
        var result = await lecturersServices.Create( new LecturerDTO
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Qualifications = vm.Qualifications,
            File = vm.File
        } );

        if ( result is null )
            return BadRequest();

        return CreatedAtAction( nameof(GetById), new { id = result.Id }, ToViewModel( result ) );
    }

    [HttpPut( "{id:guid}" )]
    public async Task<ActionResult<LecturerViewModel>> Update( Guid id, [FromForm] LecturerViewModel vm )
    {
        var existingLecturer = await lecturersServices.DetailsAsync( id );
        if ( existingLecturer is null )
            return NotFound();

        var result = await lecturersServices.Update( new LecturerDTO
        {
            Id = id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Qualifications = vm.Qualifications,
            File = vm.File
        } );

        if ( result is null )
            return BadRequest();

        return Ok( ToViewModel( result ) );
    }

    [HttpDelete( "{id:guid}" )]
    public async Task<IActionResult> Delete( Guid id )
    {
        var result = await lecturersServices.Delete( id );
        if ( result is null )
            return NotFound();

        return NoContent();
    }

    private static LecturerViewModel ToViewModel( Lecturer lecturer )
    {
        return new LecturerViewModel
        {
            Id = lecturer.Id,
            FirstName = lecturer.FirstName,
            LastName = lecturer.LastName,
            Qualifications = lecturer.Qualifications,
            ImageId = lecturer.Image.Select( image => (Guid?)image.ImageId ).FirstOrDefault(),
            CreatedAt = lecturer.CreatedAt,
            ModifiedAt = lecturer.ModifiedAt
        };
    }
}