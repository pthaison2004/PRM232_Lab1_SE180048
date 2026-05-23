using System;
using System.Collections.Generic;

namespace PRN232.Lms.Services.Models;

public class SemesterResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<CourseResponse>? Courses { get; set; }
}
