using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class LanguageCoursesServices( School2Context context ) : ILanguageCoursesServices
{
    public async Task<LanguageCourse> Create( LanguageCourseDTO dto )
    {
        if ( dto == null )
            return null;

        if ( string.IsNullOrWhiteSpace( dto.Nimetus )
             || string.IsNullOrWhiteSpace( dto.Keel ) )
            return null;

        LanguageCourse domain = new()
        {
            Id = Guid.NewGuid(),
            Nimetus = dto.Nimetus,
            Keel = dto.Keel,
            Kirjeldus = dto.Kirjeldus,
            Tase = dto.Tase,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };

        // TODO:
        // later, require user id to be attached to "ModifiedBy" parameter, to know who modified last.

        //TODO: check if db addition succeeded, if yes, return object, if not, null
        await context.LanguageCourses.AddAsync( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<LanguageCourse> Update( LanguageCourseDTO dto )
    {
        if ( dto == null )
            return null;

        if ( string.IsNullOrWhiteSpace( dto.Nimetus )
             || string.IsNullOrWhiteSpace( dto.Keel ) )
            return null;

        var any = await context.LanguageCourses.AnyAsync( x => x.Id == dto.Id );
        if ( !any )
            return null;

        var domain = new LanguageCourse();
        domain.Id = (Guid)dto.Id;
        domain.Kirjeldus = dto.Kirjeldus;
        domain.Nimetus = dto.Nimetus;
        domain.Keel = dto.Keel;
        domain.ModifiedAt = DateTime.UtcNow;
        domain.CreatedAt = (DateTime)dto.CreatedAt;
        domain.Tase = dto.Tase;

        context.ChangeTracker.Clear(); // - puhustab hetkel jälgitud konteksti
        context.LanguageCourses.Update( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<LanguageCourse> DetailsAsync( Guid id )
    {
        var result = await context.LanguageCourses
            .FirstOrDefaultAsync( x => x.Id == id );

        return result;
    }

    public async Task<LanguageCourse> Delete( Guid id )
    {
        var result = await context.LanguageCourses
            .FirstOrDefaultAsync( x => x.Id == id );

        if ( result == null )
            return null;

        context.LanguageCourses.Remove( result );
        await context.SaveChangesAsync();

        return result;
    }
}