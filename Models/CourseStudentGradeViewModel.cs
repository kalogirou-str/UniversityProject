namespace UniversityProject.Models
{
    public class CourseStudentGradeViewModel
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; }
        public string Semester { get; set; }
        public List<StudentGradeModel> Students { get; set; }
    }
}
