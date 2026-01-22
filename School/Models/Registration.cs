using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace School.Models
{
    public class Registration
    {
        public int ID { get; set; }

        public string Staatus { get; set; } = null!;

        [Required]
        public int TrainingID { get; set; }
        public virtual Training Training { get; set; } = null!;

        [Required]
        public string StudentUserID { get; set; } = null!;
        public virtual IdentityUser StudentUser { get; set; } = null!;
    }
}
