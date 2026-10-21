namespace School2.Core.Domain;

public class Lecturer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Qualifications { get; set; }

    public string? UserId { get; set; }

    //public ICollection<LanguageSubject> LanguageSubjects { get; set; }
    public IEnumerable<FileToDatabase> Image { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}