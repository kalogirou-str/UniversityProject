using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityProject.Models;

[Table("course_has_students")]
public partial class CourseHasStudent
{
    [Key]
    [Column("COURSE_idCOURSE")]
    public int CourseIdCourse { get; set; }

    [Column("STUDENTS_RegistrationNumber")]
    public int StudentsRegistrationNumber { get; set; }

    public int? GradeCourseStudent { get; set; }

    [ForeignKey("CourseIdCourse")]
    [InverseProperty("CourseHasStudent")]
    public virtual Course? CourseIdCourseNavigation { get; set; }

    [ForeignKey("StudentsRegistrationNumber")]
    [InverseProperty("CourseHasStudents")]
    public virtual Student? StudentsRegistrationNumberNavigation { get; set; }
}
