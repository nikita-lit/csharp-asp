using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class LecturersServices( School2Context context ) : ILecturersServices
{
    public async Task<Lecturer> Create( LecturerDTO dto )
    {
        if ( !IsValidLecturer( dto ) )
            return null;

        Lecturer domain = new()
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Qualifications = dto.Qualifications,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await context.Lecturers.AddAsync( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Lecturer> Update( LecturerDTO dto )
    {
        if ( !IsValidLecturer( dto ) )
            return null;

        var any = await context.Lecturers.AnyAsync( x => x.Id == dto.Id );
        if ( !any )
            return null;

        var domain = new Lecturer
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Qualifications = dto.Qualifications,
            ModifiedAt = DateTime.UtcNow,
            CreatedAt = (DateTime)dto.CreatedAt
        };

        context.ChangeTracker.Clear(); // - puhustab hetkel jälgitud konteksti
        context.Lecturers.Update( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Lecturer> DetailsAsync( Guid id )
    {
        var result = await context.Lecturers
            .FirstOrDefaultAsync( x => x.Id == id );

        return result;
    }

    public async Task<Lecturer> Delete( Guid id )
    {
        var result = await context.Lecturers
            .FirstOrDefaultAsync( x => x.Id == id );

        if ( result == null )
            return null;

        context.Lecturers.Remove( result );
        await context.SaveChangesAsync();

        return result;
    }

    private static bool IsValidLecturer( LecturerDTO? dto )
    {
        if ( dto == null )
            return false;

        if ( dto.FirstName.Length < 1 ||
             dto.LastName.Length < 1 ||
             dto.Qualifications.Length < 1 )
            return false;

        return true;
    }
}