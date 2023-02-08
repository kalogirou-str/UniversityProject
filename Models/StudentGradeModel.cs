using System.ComponentModel.DataAnnotations;

public class StudentGradeModel
{
    public int StudentRegistrationNumber { get; set; }
    public string StudentName { get; set; }
    public string StudentSurname { get; set; }
    public string CourseTitle { get; set; }
    public string CourseSemester { get; set; }
    [Range(0, 10)]
    public int? Grade { get; set; }
}
