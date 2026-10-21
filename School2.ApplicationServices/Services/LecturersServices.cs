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
        if ( dto == null )
            return null;

        Lecturer domain = new();
        domain.Id = Guid.NewGuid();

        if ( dto.FirstName.Length < 1 ||
             dto.LastName.Length < 1 ||
             dto.Qualifications.Length < 1 )
            return null;

        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Qualifications = dto.Qualifications;
        domain.CreatedAt = DateTime.UtcNow;
        domain.ModifiedAt = DateTime.UtcNow;

        await context.Lecturers.AddAsync( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Lecturer> Update( LecturerDTO dto )
    {
        if ( dto == null )
            return null;

        var any = await context.Lecturers.AnyAsync( x => x.Id == dto.Id );
        if ( !any )
            return null;

        var domain = new Lecturer();
        domain.Id = dto.Id;
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Qualifications = dto.Qualifications;
        domain.ModifiedAt = DateTime.UtcNow;
        domain.CreatedAt = (DateTime)dto.CreatedAt;

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
}