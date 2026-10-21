using School2.Core.Domain;
using School2.Core.DTO;

namespace School2.Core.ServiceInterface;

public interface ILanguageCoursesServices
{
    Task<LanguageCourse> Create( LanguageCourseDTO dto );
    Task<LanguageCourse> Update( LanguageCourseDTO dto );
    Task<LanguageCourse> DetailsAsync( Guid id );
    Task<LanguageCourse> Delete( Guid id );
}