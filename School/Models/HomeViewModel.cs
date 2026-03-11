namespace School.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Course>? Courses { get; set; }
        public IEnumerable<Training>? CurrentTrainings { get; set; }
        public IDictionary<int, int>? RegistrationCounts { get; set; }
        public IDictionary<int, string>? RegistrationStatuses { get; set; }
    }
}
