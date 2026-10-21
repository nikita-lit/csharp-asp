using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class StudentsServices : IStudentsServices
{
    private readonly School2Context _context;

    public StudentsServices(School2Context context)
    {
        _context = context;
    }
    
    public async Task<Student> Create(StudentDTO dto)
    {
        if (dto == null)
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

        await _context.Students.AddAsync(domain);
        await _context.SaveChangesAsync();
            
        return domain;
    }

    public async Task<Student> Update(StudentDTO dto)
    {
        if (dto == null)
            return null;
        
        var domain = new Student
        {
            Id = (Guid)dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            ModifiedAt = DateTime.UtcNow,
            CreatedAt = dto.CreatedAt
        };
            
        _context.ChangeTracker.Clear();
        _context.Students.Update(domain);
        await _context.SaveChangesAsync();
            
        return domain;
    }

    public async Task<Student> DetailsAsync(Guid id)
    {
        var result = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);
            
        return result;
    }

    public async Task<Student> Delete(Guid id)
    {
        var result = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);
            
        _context.Students.Remove(result);
        await _context.SaveChangesAsync();
            
        return result;
    }
}