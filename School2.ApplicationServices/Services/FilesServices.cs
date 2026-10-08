using Microsoft.Extensions.Hosting;
using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2.ApplicationServices.Services;

public class FilesServices : IFilesServices
{
    private readonly School2Context _context;
    private readonly IHostEnvironment _hostEnvironment;

    public FilesServices(School2Context context, IHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }
    
    public void UploadFiles(LecturerDTO dto, Lecturer domain)
    {
        if (dto.Files != null && dto.Files.Count > 0)
        {
            foreach (var file in dto.Files)
            {
                using (var target = new MemoryStream())
                {
                    FileToDatabase files = new()
                    {
                        ImageId = Guid.NewGuid(),
                        ImageTitle = file.FileName,
                        LecturerId = domain.Id,
                    };
                    file.CopyTo(target);
                    files.ImageData = target.ToArray();

                    _context.Files.Add(files);
                }
            }
        }
    }

    public Task<FileToDatabase> RemoveFileFromDatabase(FileToDatabaseDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<FileToDatabase> RemoveFilesFromDatabase(FileToDatabaseDTO[] dtos)
    {
        throw new NotImplementedException();
    }
}