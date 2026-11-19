using Microsoft.Extensions.Hosting;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class FilesServices( School2Context context, IHostEnvironment hostEnvironment ) : IFilesServices
{
    public void UploadFile( LecturerDTO dto, Lecturer domain )
    {
        if (dto.File == null)
            return;

        using var target = new MemoryStream();
        dto.File.CopyTo(target);

        var oldImages = context.Files.Where(x => x.LecturerId == domain.Id).ToList();
        context.Files.RemoveRange(oldImages);

        context.Files.Add(new FileToDatabase
        {
            ImageId = Guid.NewGuid(),
            ImageTitle = dto.File.FileName,
            LecturerId = domain.Id,
            ImageData = target.ToArray()
        });
    }

    public Task<FileToDatabase> RemoveFileFromDatabase( FileToDatabaseDTO dto )
    {
        throw new NotImplementedException();
    }

    public Task<FileToDatabase> RemoveFilesFromDatabase( FileToDatabaseDTO[] dtos )
    {
        throw new NotImplementedException();
    }
}
