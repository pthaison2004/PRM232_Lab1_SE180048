using System;

namespace PRN232.Lms.Services.Models;

public class EnrollmentRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;
}
