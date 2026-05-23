using System.Collections.Generic;

namespace PRN232.Lms.Services.Models;

public class CourseModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int SemesterId { get; set; }

    public SemesterModel? Semester { get; set; }
    public List<EnrollmentModel>? Enrollments { get; set; }
}
