using Microsoft.AspNetCore.Identity;

namespace School.Models
{
    public class TeacherCoursesViewModel
    {
        public Teacher Teacher { get; set; } = null!;
        public List<Training> Trainings { get; set; } = new List<Training>();
        public Dictionary<int, List<IdentityUser>> TrainingStudents { get; set; } = new Dictionary<int, List<IdentityUser>>();
    }
}
