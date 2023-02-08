using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityProject.Models;

namespace UniversityProject.Controllers
{
    public class SecretariesActions : Controller
    {
        private readonly UniversityDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SecretariesActions(IHttpContextAccessor httpContextAccessor, UniversityDBContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult RegisterCourse()
        {
            var professors = _context.Professors.Select(p => p.Afm).ToList();
            ViewBag.ProfessorsAfm = new SelectList(professors);
            return View();
        }

        [HttpPost]
        public IActionResult RegisterCourse(Course course)
        {
            if (_context.Courses.Any(c => c.IdCourse == course.IdCourse))
            {
                ModelState.AddModelError("IdCourse", "IdCourse already exists.");
            }

            if (_context.Courses.Any(c => c.CourseTitle == course.CourseTitle))
            {
                ModelState.AddModelError("CourseTitle", "CourseTitle already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                var professors = _context.Professors.Select(p => p.Afm).ToList();
                ViewBag.ProfessorsAfm = new SelectList(professors);
                return View(course);
            }
        }


        public IActionResult RegisterStudent(StudentViewModel studentViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(x => x.Username == studentViewModel.UserName))
                {
                    ModelState.AddModelError("UserName", "Username is already in use.");
                    return View(studentViewModel);
                }

                if (_context.Students.Any(x => x.RegistrationNumber == studentViewModel.RegistrationNumber))
                {
                    ModelState.AddModelError("RegistrationNumber", "Registration Number is already in use.");
                    return View(studentViewModel);
                }

                var user = new User
                {
                    Username = studentViewModel.UserName,
                    Password = studentViewModel.Password,
                    Role = "student"
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                var student = new Student
                {
                    RegistrationNumber = studentViewModel.RegistrationNumber,
                    Name = studentViewModel.Name,
                    Surname = studentViewModel.Surname,
                    Department = studentViewModel.Department,
                    UsersUsername = user.Username
                };
                _context.Students.Add(student);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(studentViewModel);
            }
        }


        public IActionResult RegisterProfessor(ProfessorViewModel professorViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(x => x.Username == professorViewModel.UserName))
                {
                    ModelState.AddModelError("UserName", "Username is already in use.");
                    return View(professorViewModel);
                }

                if (_context.Professors.Any(x => x.Afm == professorViewModel.Afm))
                {
                    ModelState.AddModelError("Afm", "Afm is already in use.");
                    return View(professorViewModel);
                }

                var user = new User
                {
                    Username = professorViewModel.UserName,
                    Password = professorViewModel.Password,
                    Role = "professor"
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                var professor = new Professor
                {
                    Afm = professorViewModel.Afm,
                    Name = professorViewModel.Name,
                    Surname = professorViewModel.Surname,
                    Department = professorViewModel.Department,
                    UsersUsername = user.Username
                };
                _context.Professors.Add(professor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(professorViewModel);
            }
        }

        public IActionResult ViewCourses()
        {
            var courses = _context.Courses
                    .Include(c => c.ProfessorsAfmNavigation)
                    .Select(c => new CourseViewModel
                    {
                        CourseTitle = c.CourseTitle,
                        Semester = c.CourseSemester,
                        ProfessorName = c.ProfessorsAfmNavigation.Name,
                        ProfessorSurname = c.ProfessorsAfmNavigation.Surname
                    }).ToList();
            return View(courses);
        }

        public IActionResult Assign()
        {
            var model = new AssignCourseViewModel
            {
                Courses = _context.Courses
                                  .Select(c => new SelectListItem { Value = c.IdCourse.ToString(), Text = c.CourseTitle }),
                Professors = _context.Professors
                                    .Select(p => new SelectListItem { Value = p.Afm.ToString(), Text = $"{p.Name} {p.Surname}" }),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Assign(int courseId, int professorAFM)
        {
            // Validate input data
            if (courseId < 0 || professorAFM < 0)
            {
                return View("Index");
            }

            var course = _context.Courses.FirstOrDefault(c => c.IdCourse == courseId);
            var professor = _context.Professors.FirstOrDefault(p => p.Afm == professorAFM);


            // Assign course to professor
            course.ProfessorsAfm = professor.Afm;
            _context.SaveChanges();

            return View("Index");
        }

        public IActionResult Declare()
        {
            var courses = _context.Courses.ToList();
            var students = _context.Students.Select(x => new SelectListItem { Value = x.RegistrationNumber.ToString(), Text = x.Name + " " + x.Surname }).ToList();

            var viewModel = new DeclareCourseViewModel()
            {
                Courses = courses,
                Students = students,
                Grade = null
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Declare(DeclareCourseViewModel viewModel)
        {
            var courseHasStudent = new CourseHasStudent()
            {
                CourseIdCourse = viewModel.SelectedCourseId,
                StudentsRegistrationNumber = Convert.ToInt32(viewModel.SelectedStudentId),
                GradeCourseStudent = null
            };
            _context.CourseHasStudents.Add(courseHasStudent);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}


