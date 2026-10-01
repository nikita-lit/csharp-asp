using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace School2.ApplicationServices.Services
{
    public class LanguageCoursesServices : ILanguageCoursesServices
    {
        private readonly School2Context _context;

        public LanguageCoursesServices(School2Context context)
        {
            _context = context;
        }

        public async Task<LanguageCourse> Create(LanguageCourseDTO dto)
        {
            return null;
        }
        public async Task<LanguageCourse> Update(LanguageCourseDTO dto)
        {
            return null;
        }
        public async Task<LanguageCourse> Update(Guid id)
        {
            return null;
        }
        public async Task<LanguageCourse> Delete(Guid id)
        {
            return null;
        }
    }
}
