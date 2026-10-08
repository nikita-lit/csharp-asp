using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class LecturersServices : ILecturersServices
{
    private readonly School2Context _context;

    public LecturersServices(School2Context context)
    {
        _context = context;
    }
    
    public async Task<Lecturer> Create(LecturerDTO dto)
    {
        if (dto == null)
            return null;

        Lecturer domain = new();
        domain.Id = Guid.NewGuid();
            
        if (dto.FirstName.Length < 1 || 
            dto.LastName.Length < 1 ||
            dto.Qualifications.Length < 1)
            return null;
            
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Qualifications = dto.Qualifications;
        domain.CreatedAt = DateTime.UtcNow;
        domain.ModifiedAt = DateTime.UtcNow;
        
        await _context.Lecturers.AddAsync(domain);
        await _context.SaveChangesAsync();
            
        return domain;
    }

    public async Task<Lecturer> Update(LecturerDTO dto)
    {
        if (dto == null)
            return null;
        
        var domain = new Lecturer();
        domain.Id = dto.Id;
        domain.FirstName = dto.FirstName;
        domain.LastName = dto.LastName;
        domain.Qualifications = dto.Qualifications;
        domain.ModifiedAt = DateTime.UtcNow;
        domain.CreatedAt = (DateTime)dto.CreatedAt;
            
        _context.ChangeTracker.Clear();// - puhustab hetkel jälgitud konteksti
        _context.Lecturers.Update(domain);
        await _context.SaveChangesAsync();
            
        return domain;
    }

    public async Task<Lecturer> DetailsAsync(Guid id)
    {
        var result = await _context.Lecturers
            .FirstOrDefaultAsync(x => x.Id == id);
            
        return result;
    }

    public async Task<Lecturer> Delete(Guid id)
    {
        var result = await _context.Lecturers
            .FirstOrDefaultAsync(x => x.Id == id);
            
        _context.Lecturers.Remove(result);
        await _context.SaveChangesAsync();
            
        return result;
    }
}