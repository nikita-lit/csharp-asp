using System.ComponentModel.DataAnnotations;

namespace School2.Models.LanguageCourses;

public class LanguageCourseCreateUpdateViewModel
{
    public Guid? Id { get; set; } // optional sest index vaade ei vaja seda

    [Required]
    [StringLength( 100 )]
    public string Nimetus { get; set; }

    [Required]
    [StringLength( 100 )]
    public string Keel { get; set; }

    [StringLength( 100 )]
    public string? Tase { get; set; } // optional sest index vaade ei vaja seda

    [StringLength( 500 )]
    public string? Kirjeldus { get; set; } // optional sest index vaade ei vaja seda

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}