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
            if (dto == null)
                return null;

            LanguageCourse domain = new();
            domain.Id = Guid.NewGuid();
            domain.Nimetus = "";
            domain.Keel = "";
            
            if (domain.Nimetus.Length < 1)
                return null;

            if (domain.Keel.Length < 1)
                return null;
            
            domain.Kirjeldus = dto.Kirjeldus;
            domain.Tase = dto.Tase;
            domain.CreatedAt = DateTime.Now;
            domain.ModifiedAt = DateTime.Now;
            // TODO:
            // later, require user id to be attached to "ModifiedBy" parameter, to know who modified last.
           
            //TODO: check if db addition succeeded, if yes, return object, if not, null
            await _context.LanguageCourses.AddAsync(domain);
            await _context.SaveChangesAsync();
            
            return domain;
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
