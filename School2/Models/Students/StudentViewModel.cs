using System.ComponentModel.DataAnnotations;

namespace School2.Models.Students;

public class StudentViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength( 100 )]
    public string FirstName { get; set; }

    [Required]
    [StringLength( 100 )]
    public string LastName { get; set; }

    [Required]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}