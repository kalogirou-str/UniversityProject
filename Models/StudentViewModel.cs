using System.ComponentModel.DataAnnotations;

namespace UniversityProject.Models
{
    public class StudentViewModel
    {
        [Required]
        public int RegistrationNumber { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
