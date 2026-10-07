using School2.Core.Domain;
using School2.Core.DTO;
using School2.Core.ServiceInterface;
using School2.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

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
            
            if (dto.Nimetus.Length < 1)
                return null;

            if (dto.Keel.Length < 1)
                return null;
            
            domain.Nimetus = dto.Nimetus;
            domain.Keel = dto.Keel;
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
            var domain = new LanguageCourse();
            domain.Id = (Guid)dto.Id;
            domain.Kirjeldus = dto.Kirjeldus;
            domain.Nimetus = dto.Nimetus;
            domain.Keel = dto.Keel;
            domain.ModifiedAt = DateTime.UtcNow;
            domain.CreatedAt = (DateTime)dto.CreatedAt;
            domain.Tase = dto.Tase;
            
            _context.ChangeTracker.Clear();// - puhustab hetkel jälgitud konteksti
            _context.LanguageCourses.Update(domain);
            await _context.SaveChangesAsync();
            
            return domain;
        }
        
        public async Task<LanguageCourse> DetailsAsync(Guid id)
        {
            var result = await _context.LanguageCourses
                .FirstOrDefaultAsync(x => x.Id == id);
            
            return result;
        }
        
        public async Task<LanguageCourse> Delete(Guid id)
        {
            return null;
        }
    }
}
