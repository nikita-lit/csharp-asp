using System.ComponentModel.DataAnnotations;

namespace School2.Models.Lecturers;

public class LecturerViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Qualifications { get; set; } = string.Empty;

    public List<IFormFile>? Files { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}