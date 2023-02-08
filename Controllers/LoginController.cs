using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using UniversityProject.Controllers;
using UniversityProject.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LoginPage.Controllers
{
    public class LoginController : Controller
    {

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // check if the provided username and password match with the database
            // use the Entity framework to check the database
            using (UniversityDBContext db = new UniversityDBContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    ViewBag.Role = user.Role;
                    // Serialize the user object
                    var json = JsonConvert.SerializeObject(user);
                    byte[] userData = Encoding.UTF8.GetBytes(json);
                    HttpContext.Session.Set("user", userData);
                    // redirect the user to the appropriate page based on their role
                    switch (user.Role)
                    {
                        case "student":
                            var student = db.Students.FirstOrDefault(s => s.UsersUsername == user.Username);
                            int registrationNumber = student.RegistrationNumber;
                            HttpContext.Session.SetInt32("registrationNumber", registrationNumber);
                            return RedirectToAction("Index", "StudentsActions");
                        case "professor":
                            var professor = db.Professors.FirstOrDefault(p => p.UsersUsername == user.Username);
                            int afm = professor.Afm;
                            HttpContext.Session.SetInt32("afm", afm);
                            return RedirectToAction("Index", "ProfessorsActions");
                        case "secretary":
                            var secretary = db.Secretaries.FirstOrDefault(s => s.UsersUsername == user.Username);
                            int phoneNumber = secretary.Phonenumber;
                            HttpContext.Session.SetInt32("phoneNumber", phoneNumber);
                            return RedirectToAction("Index", "SecretariesActions");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // if no match is found, return an error message
                    TempData["Message"] = "Incorrect username or password";
                    return RedirectToAction("Index");
                }
            }
        }
    }
}