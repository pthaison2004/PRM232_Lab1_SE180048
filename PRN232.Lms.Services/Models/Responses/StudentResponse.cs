using System;
using System.Collections.Generic;

namespace PRN232.Lms.Services.Models;

public class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    public List<EnrollmentResponse>? Enrollments { get; set; }
}
