using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace School.Models
{
    public class NotifyViewModel
    {
        public int? TrainingId { get; set; }
        public string? StudentUserId { get; set; }

        public List<SelectListItem> Trainings { get; set; } = new();
        public List<SelectListItem> Students { get; set; } = new();

        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
