using School2.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace School2.Data
{
    public class School2Context : IdentityDbContext<ApplicationUser>
    {
        //set tables here
        public DbSet<LanguageCourse> LanguageCourses { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<FileToDatabase> Files { get; set; }
        
        public School2Context(DbContextOptions<School2Context> options) : base (options) 
        {
        }
    }
}
