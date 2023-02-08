using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityProject.Models;

namespace UniversityProject.Controllers
{
    public class Secretaries2 : Controller
    {
        private readonly UniversityDBContext _context;

        public Secretaries2(UniversityDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,Role,RegistrationNumber,Name,Surname,Department")]
        SecretaryInstertsStudent viewModel)
        {
            if (ModelState.IsValid)
            {
                // Create new user and student objects
                var user = new User
                {
                    Username = viewModel.Username,
                    Password = viewModel.Password,
                    Role = viewModel.Role
                };

                var student = new Student
                {
                    RegistrationNumber = Int32.Parse(viewModel.RegistrationNumber) ,
                    Name = viewModel.Name,
                    Surname = viewModel.Surname,
                    Department = viewModel.Department,
                    UsersUsername = viewModel.Username
                };

                // Add the new user and student to the database
                using (var context = new UniversityDBContext())
                {
                    context.Users.Add(user);
                    context.Students.Add(student);
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            return View(viewModel);
        }
    }
}