using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace UniversityProject.Models
{
    public class AssignCourseViewModel
    {
        public int CourseId { get; set; }
        public int ProfessorAFM { get; set; }
        public IEnumerable<SelectListItem> Courses { get; set; }
        public IEnumerable<SelectListItem> Professors { get; set; }
    }
}