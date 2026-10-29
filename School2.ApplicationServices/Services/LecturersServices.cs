using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class LecturersServices(
    School2Context context,
    IFilesServices filesServices) : ILecturersServices
{
    public async Task<Lecturer?> Create(LecturerDTO? dto)
    {
        if (!IsValidLecturer(dto))
            return null;

        Lecturer domain = new()
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Qualifications = dto.Qualifications.Trim(),
            UserId = dto.UserId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await context.Lecturers.AddAsync(domain);

        filesServices.UploadFiles(dto, domain);

        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Lecturer?> Update(LecturerDTO? dto)
    {
        if (!IsValidLecturer(dto))
            return null;

        var domain = await context.Lecturers
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (domain is null)
            return null;

        domain.FirstName = dto.FirstName.Trim();
        domain.LastName = dto.LastName.Trim();
        domain.Qualifications = dto.Qualifications.Trim();
        domain.ModifiedAt = DateTime.UtcNow;

        filesServices.UploadFiles(dto, domain);

        await context.SaveChangesAsync();

        return domain;
    }

    public async Task<Lecturer?> DetailsAsync(Guid id)
    {
        var result = await context.Lecturers
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == id);

        return result;
    }

    public async Task<Lecturer?> Delete(Guid id)
    {
        var result = await context.Lecturers
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (result is null)
            return null;

        // Сначала удаляем связанные файлы.
        context.Files.RemoveRange(result.Image);

        context.Lecturers.Remove(result);

        await context.SaveChangesAsync();

        return result;
    }

    private static bool IsValidLecturer(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] LecturerDTO? dto)
    {
        return dto is not null
            && !string.IsNullOrWhiteSpace(dto.FirstName)
            && !string.IsNullOrWhiteSpace(dto.LastName)
            && !string.IsNullOrWhiteSpace(dto.Qualifications);
    }
}