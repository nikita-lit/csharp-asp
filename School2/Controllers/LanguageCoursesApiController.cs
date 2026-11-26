using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.Models.LanguageCourses;

namespace School2.Controllers;

[ApiController, Route( "api/language-courses" )]
public class LanguageCoursesApiController(
    School2Context context,
    ILanguageCoursesServices languageCoursesServices ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LanguageCourseViewModel>>> GetAll()
    {
        var courses = await context.LanguageCourses
            .Select( course => new LanguageCourseViewModel
            {
                Id = course.Id,
                Nimetus = course.Nimetus,
                Keel = course.Keel,
                Tase = course.Tase,
                Kirjeldus = course.Kirjeldus
            } )
            .Take( 20 )
            .ToListAsync();

        return Ok( courses );
    }

    [HttpGet( "{id:guid}" )]
    public async Task<ActionResult<LanguageCourseViewModel>> GetById( Guid id )
    {
        var course = await languageCoursesServices.DetailsAsync( id );
        if ( course is null )
            return NotFound();

        return Ok( ToViewModel( course ) );
    }

    [HttpPost]
    public async Task<ActionResult<LanguageCourseViewModel>> Create( LanguageCourseCreateUpdateViewModel vm )
    {
        var result = await languageCoursesServices.Create( new LanguageCourseDTO
        {
            Nimetus = vm.Nimetus,
            Keel = vm.Keel,
            Tase = vm.Tase,
            Kirjeldus = vm.Kirjeldus
        } );

        if ( result is null )
            return BadRequest();

        return CreatedAtAction( nameof(GetById), new { id = result.Id }, ToViewModel( result ) );
    }

    [HttpPut( "{id:guid}" )]
    public async Task<ActionResult<LanguageCourseViewModel>> Update(
        Guid id,
        LanguageCourseCreateUpdateViewModel vm )
    {
        var existingCourse = await languageCoursesServices.DetailsAsync( id );
        if ( existingCourse is null )
            return NotFound();

        var result = await languageCoursesServices.Update( new LanguageCourseDTO
        {
            Id = id,
            Nimetus = vm.Nimetus,
            Keel = vm.Keel,
            Tase = vm.Tase,
            Kirjeldus = vm.Kirjeldus,
            CreatedAt = existingCourse.CreatedAt
        } );

        if ( result is null )
            return BadRequest();

        return Ok( ToViewModel( result ) );
    }

    [HttpDelete( "{id:guid}" )]
    public async Task<IActionResult> Delete( Guid id )
    {
        var result = await languageCoursesServices.Delete( id );
        if ( result is null )
            return NotFound();

        return NoContent();
    }

    private static LanguageCourseViewModel ToViewModel( LanguageCourse course )
    {
        return new LanguageCourseViewModel
        {
            Id = course.Id,
            Nimetus = course.Nimetus,
            Keel = course.Keel,
            Tase = course.Tase,
            Kirjeldus = course.Kirjeldus
        };
    }
}