using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace School.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;

        [Required]
        public int TrainingId { get; set; }
        public virtual Training Training { get; set; } = null!;

        [Required]
        public string StudentUserId { get; set; } = null!;
        public virtual IdentityUser StudentUser { get; set; } = null!;
    }
}
