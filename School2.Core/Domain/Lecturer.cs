namespace School2.Core.Domain;

public class Lecturer
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string Qualifications { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public ICollection<FileToDatabase> Image { get; set; } = new List<FileToDatabase>();

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}