namespace UniversityProject.Models
{
    public class GradeViewModel
    {
        public string CourseTitle { get; set; }
        public string Semester { get; set; }
        public List<GradeModel> Grades { get; set; }
    }
}
