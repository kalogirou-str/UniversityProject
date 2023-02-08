using Microsoft.AspNetCore.Mvc;
using UniversityProject.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace UniversityProject.Controllers
{
    public class ProfessorsActions : Controller
    {
        private readonly UniversityDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ProfessorsActions(IHttpContextAccessor httpContextAccessor, UniversityDBContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public IActionResult Index()
        {
            int afm = (int)_httpContextAccessor.HttpContext.Session.GetInt32("afm");
            var professor = _context.Professors.SingleOrDefault(s => s.Afm == afm);
            string professorName = professor.UsersUsername;
            return View("Index", professorName);
        }

        public IActionResult ViewGrades()
        {
            int afm = (int)_httpContextAccessor.HttpContext.Session.GetInt32("afm");
            // Retrieve courses associated with the professor using the AFM
            var courses = _context.Courses.Where(c => c.ProfessorsAfm == afm).ToList();

            // Create a list of view models to pass to the view
            var viewModels = new List<GradeViewModel>();
            foreach (var course in courses)
            {
                var viewModel = new GradeViewModel
                {
                    CourseTitle = course.CourseTitle,
                    Semester = course.CourseSemester,
                    Grades = _context.CourseHasStudents
                                .Where(c => c.CourseIdCourse == course.IdCourse && c.GradeCourseStudent!=null)
                                .Select(c => new GradeModel
                                {
                                    StudentName = c.StudentsRegistrationNumberNavigation.Name,
                                    StudentSurname = c.StudentsRegistrationNumberNavigation.Surname,
                                    StudentRegistrationNumber = c.StudentsRegistrationNumberNavigation.RegistrationNumber,
                                    Grade = c.GradeCourseStudent
                                }).ToList()
                };
                viewModels.Add(viewModel);
            }
            return View(viewModels);
        }

        public IActionResult EditGrades()
        {
            int afm = (int)_httpContextAccessor.HttpContext.Session.GetInt32("afm");
            // Retrieve the list of courses associated with the current professor using the afm and the appropriate navigation properties
            var courses = _context.Courses.Where(c => c.ProfessorsAfm == afm).ToList();

            // Create a list of view models to pass to the view
            var viewModels = new List<CourseStudentGradeViewModel>();
            foreach (var course in courses)
            {
                var viewModel = new CourseStudentGradeViewModel
                {
                    Id = course.IdCourse,
                    CourseTitle = course.CourseTitle,
                    Semester = course.CourseSemester,
                    Students = _context.CourseHasStudents
                                .Where(c => c.CourseIdCourse == course.IdCourse && c.GradeCourseStudent == null)
                                .Select(c => new StudentGradeModel
                                {
                                    StudentName = c.StudentsRegistrationNumberNavigation.Name,
                                    StudentSurname = c.StudentsRegistrationNumberNavigation.Surname,
                                    StudentRegistrationNumber = c.StudentsRegistrationNumberNavigation.RegistrationNumber,
                                    Grade = c.GradeCourseStudent
                                }).ToList()
                };
                viewModels.Add(viewModel);
            }
            return View(viewModels);
        }

        [HttpPost]
        public IActionResult EditChanges(List<CourseStudentGradeViewModel> viewModels)
        {
            // Iterate over the view models to retrieve the new grades
            foreach (var viewModel in viewModels)
            {
                foreach (var student in viewModel.Students)
                {
                    // Retrieve the corresponding record from the course_has_students table
                    var courseHasStudent = _context.CourseHasStudents
                        .FirstOrDefault(c => c.CourseIdCourse == viewModel.Id && c.StudentsRegistrationNumber == student.StudentRegistrationNumber);

                    // Update the grade
                    courseHasStudent.GradeCourseStudent = student.Grade;
                }
            }

            // Save the changes to the database
            _context.SaveChanges();

            return RedirectToAction("EditGrades");
        }

    }
}
