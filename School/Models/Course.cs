using System.ComponentModel.DataAnnotations;

namespace School.Models
{
    public class Course
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Level { get; set; } = null!;

        public string? Description { get; set; }
    }
}
