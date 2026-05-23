using System.Collections.Generic;

namespace PRN232.Lms.Services.Models;

public class CourseResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }

    public SemesterResponse? Semester { get; set; }
    public List<EnrollmentResponse>? Enrollments { get; set; }
}
