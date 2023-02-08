using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityProject.Models;
using Microsoft.AspNetCore.Http;

namespace UniversityProject.Controllers
{
    public class StudentsActions : Controller
    {
        private readonly UniversityDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public StudentsActions(IHttpContextAccessor httpContextAccessor, UniversityDBContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public IActionResult Index()
        {
            int registrationNumber = (int)_httpContextAccessor.HttpContext.Session.GetInt32("registrationNumber");
            var student = _context.Students.SingleOrDefault(s => s.RegistrationNumber == registrationNumber);
            string studentName = student.UsersUsername;
            return View("Index", studentName);
        }

        public IActionResult ViewGradesByCourse()
        {
            int registrationNumber = (int)_httpContextAccessor.HttpContext.Session.GetInt32("registrationNumber");
            var studentGrades = _context.CourseHasStudents
                .Where(chs => chs.StudentsRegistrationNumber == registrationNumber && chs.GradeCourseStudent.HasValue)
                .Select(chs => new StudentGradesByCourseViewModel
                {
                    CourseTitle = chs.CourseIdCourseNavigation.CourseTitle,
                    CourseSemester = chs.CourseIdCourseNavigation.CourseSemester ?? "N/A",
                    Grade = chs.GradeCourseStudent,
                    StudentName = chs.StudentsRegistrationNumberNavigation.Name,
                    StudentSurname = chs.StudentsRegistrationNumberNavigation.Surname
                })
                .ToList();

            return View(studentGrades);
        }

        public ActionResult ViewGradesBySemester()
        {
            int registrationNumber = (int)_httpContextAccessor.HttpContext.Session.GetInt32("registrationNumber");
            // Use EF to retrieve the student's courses and grades from the database, grouped by semester
            using (var context = new UniversityDBContext())
            {
                var studentGrades = context.CourseHasStudents
                    .Where(chs => chs.StudentsRegistrationNumber == registrationNumber && chs.GradeCourseStudent.HasValue)
                    .Include(chs => chs.CourseIdCourseNavigation)
                    .GroupBy(chs => chs.CourseIdCourseNavigation.CourseSemester)
                    .Select(g => new StudentGradesBySemesterViewModel
                    {
                        Semester = g.Key,
                        Courses = g.Select(chs => new StudentGradesByCourseViewModel
                        {
                            CourseTitle = chs.CourseIdCourseNavigation.CourseTitle,
                            Grade = chs.GradeCourseStudent
                        }).ToList()
                    }).ToList();


                return View(studentGrades);
            }
        }

        public ActionResult ViewTotalScore()
        {
            // Get the logged-in student's registration number
            int registrationNumber = (int)_httpContextAccessor.HttpContext.Session.GetInt32("registrationNumber");

            // Use EF to retrieve the student's courses and grades from the database
            using (var context = new UniversityDBContext())
            {
                var studentGrades = context.CourseHasStudents
                    .Where(chs => chs.StudentsRegistrationNumber == registrationNumber)
                    .Select(chs => new {
                        Grade = chs.GradeCourseStudent
                    }).ToList();

                // Calculate the total score
                decimal totalScore = studentGrades.Where(grade => grade.Grade.HasValue).Sum(grade => grade.Grade.Value);
                //Calculate the average score
                int numberOfGrades = studentGrades.Count();
                decimal averageScore = totalScore / numberOfGrades;

                return View(averageScore);
            }
        }

    }
}
