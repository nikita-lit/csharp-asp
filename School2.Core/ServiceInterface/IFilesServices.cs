using School2.Core.Domain;
using School2.Core.DTO;

namespace School2.Core.ServiceInterface;

public interface IFilesServices
{
    void UploadFiles(LecturerDTO dto, Lecturer domain);
    Task<FileToDatabase> RemoveFileFromDatabase(FileToDatabaseDTO dto);
    Task<FileToDatabase> RemoveFilesFromDatabase(FileToDatabaseDTO[] dtos);
}