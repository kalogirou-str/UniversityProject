using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityProject.Models;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace UniversityProject.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public StudentsController(IHttpContextAccessor httpContextAccessor, UniversityDBContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var universityDBContext = _context.Students.Include(s => s.UsersUsernameNavigation);
            return View(await universityDBContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var modelStateVal in ViewData.ModelState.Values)
                {
                    foreach (var error in modelStateVal.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        // You may log the errors if you want
                    }
                }
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        {
            if (id != student.RegistrationNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.RegistrationNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'UniversityDBContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.RegistrationNumber == id);
        }




        public IActionResult ViewGradesByCourse()
        {
            int registrationNumber = (int)_httpContextAccessor.HttpContext.Session.GetInt32("registrationNumber");
            var studentGrades = _context.CourseHasStudents
                .Where(chs => chs.StudentsRegistrationNumber == registrationNumber)
                .Select(chs => new StudentGradesByCourseViewModel
                {
                    CourseTitle = chs.CourseIdCourseNavigation.CourseTitle,
                    CourseSemester = chs.CourseIdCourseNavigation.CourseSemester ?? "N/A",
                    Grade = chs.GradeCourseStudent
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
                    .Where(chs => chs.StudentsRegistrationNumber == registrationNumber)
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

                return View(totalScore);
            }
        }
    }
}
