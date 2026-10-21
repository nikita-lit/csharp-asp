using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class StudentsServices( School2Context context ) : IStudentsServices
{
    public async Task<Student> Create( StudentDTO dto )
    {
        if ( dto == null )
            return null;

        Student domain = new()
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await context.Students.AddAsync( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Student> Update( StudentDTO dto )
    {
        if ( dto == null )
            return null;

        // check that we have student with same id
        var any = await context.Students.AnyAsync( x => x.Id == dto.Id );
        if ( !any )
            return null;

        var domain = new Student
        {
            Id = (Guid)dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            ModifiedAt = DateTime.UtcNow,
            CreatedAt = dto.CreatedAt
        };

        context.ChangeTracker.Clear();
        context.Students.Update( domain );
        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Student> DetailsAsync( Guid id )
    {
        var result = await context.Students
            .FirstOrDefaultAsync( x => x.Id == id );

        return result;
    }

    public async Task<Student> Delete( Guid id )
    {
        var result = await context.Students
            .FirstOrDefaultAsync( x => x.Id == id );

        if ( result == null )
            return null;

        context.Students.Remove( result );
        await context.SaveChangesAsync();

        return result;
    }
}