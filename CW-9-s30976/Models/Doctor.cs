﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CW_9_s30976.Models;

public class Doctor {
    [Key]
    public int IdDoctor { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public ICollection<Prescription> Prescriptions { get; set; }
}