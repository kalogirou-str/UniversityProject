using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityProject.Models;

public class DeclareCourseViewModel
{
    public List<Course> Courses { get; set; }
    public List<SelectListItem> Students { get; set; }
    public int SelectedCourseId { get; set; }
    public int SelectedStudentId { get; set; }
    public int? Grade { get; set; }
}