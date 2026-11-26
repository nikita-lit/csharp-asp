using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.Students;

namespace School2.Controllers;

[ApiController, Route( "api/students" )]
public class StudentsApiController( School2Context context, IStudentsServices studentsServices ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentViewModel>>> GetAll()
    {
        var students = await context.Students
            .Select( student => new StudentViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                PhoneNumber = student.PhoneNumber,
                Email = student.Email,
                CreatedAt = student.CreatedAt,
                ModifiedAt = student.ModifiedAt
            } )
            .Take( 20 )
            .ToListAsync();

        return Ok( students );
    }

    [HttpGet( "{id:guid}" )]
    public async Task<ActionResult<StudentViewModel>> GetById( Guid id )
    {
        var student = await studentsServices.DetailsAsync( id );
        if ( student is null )
            return NotFound();

        return Ok( ToViewModel( student ) );
    }

    [HttpPost]
    public async Task<ActionResult<StudentViewModel>> Create( StudentViewModel vm )
    {
        var result = await studentsServices.Create( new StudentDTO
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            PhoneNumber = vm.PhoneNumber!,
            Email = vm.Email!
        } );

        if ( result is null )
            return BadRequest();

        return CreatedAtAction( nameof(GetById), new { id = result.Id }, ToViewModel( result ) );
    }

    [HttpPut( "{id:guid}" )]
    public async Task<ActionResult<StudentViewModel>> Update( Guid id, StudentViewModel vm )
    {
        var existingStudent = await studentsServices.DetailsAsync( id );
        if ( existingStudent is null )
            return NotFound();

        var result = await studentsServices.Update( new StudentDTO
        {
            Id = id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            PhoneNumber = vm.PhoneNumber!,
            Email = vm.Email!,
            CreatedAt = existingStudent.CreatedAt
        } );

        if ( result is null )
            return BadRequest();

        return Ok( ToViewModel( result ) );
    }

    [HttpDelete( "{id:guid}" )]
    public async Task<IActionResult> Delete( Guid id )
    {
        var result = await studentsServices.Delete( id );
        if ( result is null )
            return NotFound();

        return NoContent();
    }

    private static StudentViewModel ToViewModel( Student student )
    {
        return new StudentViewModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            PhoneNumber = student.PhoneNumber,
            Email = student.Email,
            CreatedAt = student.CreatedAt,
            ModifiedAt = student.ModifiedAt
        };
    }
}