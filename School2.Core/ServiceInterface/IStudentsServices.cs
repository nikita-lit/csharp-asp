using School2.Core.Domain;
using School2.Core.DTO;

namespace School2.Core.ServiceInterface;

public interface IStudentsServices
{
    Task<Student> Create( StudentDTO dto );
    Task<Student> Update( StudentDTO dto );
    Task<Student> DetailsAsync( Guid id );
    Task<Student> Delete( Guid id );
}