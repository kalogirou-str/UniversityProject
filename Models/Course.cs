using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityProject.Models;

[Table("course")]
public partial class Course
{
    [Key]
    [Column("idCOURSE")]
    public int IdCourse { get; set; }

    [StringLength(60)]
    [Unicode(false)]
    public string CourseTitle { get; set; } = null!;

    [StringLength(25)]
    [Unicode(false)]
    public string CourseSemester { get; set; } = null!;

    [Column("PROFESSORS_AFM")]
    public int ProfessorsAfm { get; set; }

    [InverseProperty("CourseIdCourseNavigation")]
    public virtual CourseHasStudent? CourseHasStudent { get; set; }

    [ForeignKey("ProfessorsAfm")]
    [InverseProperty("Courses")]
    public virtual Professor? ProfessorsAfmNavigation { get; set; }
}
