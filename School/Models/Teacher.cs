using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace School.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Qualification { get; set; }

        public string? FotoPath { get; set; }

        [Required]
        public string IdentityUserId { get; set; } = null!;
        public virtual IdentityUser? IdentityUser { get; set; }
    }
}
