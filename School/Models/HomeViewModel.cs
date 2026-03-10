using System.Collections.Generic;

namespace School.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Course>? Courses { get; set; }
        public IEnumerable<Training>? CurrentTrainings { get; set; }
        public IDictionary<int,int>? RegistrationCounts { get; set; }
    }
}
