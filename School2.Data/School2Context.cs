using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using School2.Core.Domain;

namespace School2.Data;

public class School2Context : IdentityDbContext<ApplicationUser>
{
    public School2Context( DbContextOptions<School2Context> options ) : base( options )
    {
    }

    //set tables here
    public DbSet<LanguageCourse> LanguageCourses { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<FileToDatabase> Files { get; set; }
    public DbSet<Student> Students { get; set; }
}