using System.ComponentModel.DataAnnotations;

namespace School.Models
{
    public class Training
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public float Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public int TeacherId { get; set; }
        public virtual Teacher? Teacher { get; set; }
    }
}
