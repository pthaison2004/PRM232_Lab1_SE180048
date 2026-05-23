using System;

namespace PRN232.Lms.Services.Models;

public class SemesterRequest
{
    public string SemesterName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
