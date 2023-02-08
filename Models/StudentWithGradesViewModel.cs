namespace UniversityProject.Models
{
    public class StudentWithGradesViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }

}
