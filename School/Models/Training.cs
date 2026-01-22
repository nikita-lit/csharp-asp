using System.ComponentModel.DataAnnotations;

namespace School.Models
{
    public class Training
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public float Price { get; set; }

        public DateTime AlgusKuupaev { get; set; }

        public DateTime LoppKuupaev { get; set; }

        [Required]
        public int MaxOsalejaid { get; set; }

        public int CourseID { get; set; }

        public int TeacherID { get; set; }
    }
}
