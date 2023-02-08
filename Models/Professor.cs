﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityProject.Models;

[Table("professors")]
public partial class Professor
{
    [Key]
    [Column("AFM")]
    public int Afm { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(45)]
    [Unicode(false)]
    public string Surname { get; set; } = null!;

    [StringLength(45)]
    [Unicode(false)]
    public string Department { get; set; } = null!;

    [Column("USERS_username")]
    [StringLength(45)]
    [Unicode(false)]
    public string UsersUsername { get; set; } = null!;

    [InverseProperty("ProfessorsAfmNavigation")]
    public virtual ICollection<Course> Courses { get; } = new List<Course>();

    [ForeignKey("UsersUsername")]
    [InverseProperty("Professors")]
    public virtual User? UsersUsernameNavigation { get; set; }
}
