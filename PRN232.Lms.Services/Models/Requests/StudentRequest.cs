using System;

namespace PRN232.Lms.Services.Models;

public class StudentRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
}
