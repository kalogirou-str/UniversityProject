namespace UniversityProject.Models
{
    public class StudentGradesBySemesterViewModel
    {
        public string Semester { get; set; }
        public List<StudentGradesByCourseViewModel> Courses { get; set; }
    }
}
