using System;

namespace PRN232.Lms.Services.Models;

public class EnrollmentModel
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = null!;

    public StudentModel? Student { get; set; }
    public CourseModel? Course { get; set; }
}
