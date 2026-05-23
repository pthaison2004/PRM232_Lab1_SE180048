using System;
using System.Collections.Generic;
using System.Linq;
using PRN232.Lms.Repositories.Entities;

namespace PRN232.Lms.Repositories.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // 1. Seed Semesters (Minimum 5)
        if (!context.Semesters.Any())
        {
            var semesters = new List<Semester>
            {
                new() { SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 31) },
                new() { SemesterName = "Spring 2025", StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 4, 30) },
                new() { SemesterName = "Summer 2025", StartDate = new DateTime(2025, 5, 1), EndDate = new DateTime(2025, 8, 31) },
                new() { SemesterName = "Fall 2025", StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2025, 12, 31) },
                new() { SemesterName = "Spring 2026", StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 4, 30) }
            };
            context.Semesters.AddRange(semesters);
            context.SaveChanges();
        }

        // 2. Seed Subjects (Minimum 10)
        if (!context.Subjects.Any())
        {
            var subjects = new List<Subject>
            {
                new() { SubjectCode = "PRN232", SubjectName = "Building Cross-Platform Back-End Application With .NET", Credit = 3 },
                new() { SubjectCode = "PRN211", SubjectName = "Basic Cross-Platform Application Programming With .NET", Credit = 3 },
                new() { SubjectCode = "PRM392", SubjectName = "Mobile Programming", Credit = 3 },
                new() { SubjectCode = "SWD392", SubjectName = "Software Architecture and Design", Credit = 3 },
                new() { SubjectCode = "SWT301", SubjectName = "Software Testing", Credit = 3 },
                new() { SubjectCode = "SWR302", SubjectName = "Software Requirements", Credit = 3 },
                new() { SubjectCode = "SWE201", SubjectName = "Introduction to Software Engineering", Credit = 3 },
                new() { SubjectCode = "MAD101", SubjectName = "Discrete Mathematics", Credit = 3 },
                new() { SubjectCode = "PRO192", SubjectName = "Object-Oriented Programming (Java)", Credit = 3 },
                new() { SubjectCode = "CSD201", SubjectName = "Data Structures and Algorithms", Credit = 3 }
            };
            context.Subjects.AddRange(subjects);
            context.SaveChanges();
        }

        // 3. Seed Courses (Minimum 20)
        if (!context.Courses.Any())
        {
            var semesters = context.Semesters.ToList();
            var courses = new List<Course>();
            var courseNames = new[]
            {
                "PRN232_SE1801", "PRN232_SE1802", "PRN211_SE1801", "PRN211_SE1802",
                "PRM392_SE1801", "PRM392_SE1802", "SWD392_SE1801", "SWD392_SE1802",
                "SWT301_SE1801", "SWT301_SE1802", "SWR302_SE1801", "SWR302_SE1802",
                "SWE201_SE1801", "SWE201_SE1802", "MAD101_SE1801", "MAD101_SE1802",
                "PRO192_SE1801", "PRO192_SE1802", "CSD201_SE1801", "CSD201_SE1802"
            };

            for (int i = 0; i < courseNames.Length; i++)
            {
                var semester = semesters[i % semesters.Count];
                courses.Add(new Course
                {
                    CourseName = courseNames[i],
                    SemesterId = semester.SemesterId
                });
            }
            context.Courses.AddRange(courses);
            context.SaveChanges();
        }

        // 4. Seed Students (Minimum 50)
        if (!context.Students.Any())
        {
            var firstNames = new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Huynh", "Phan", "Vu", "Vo", "Dang" };
            var middleNames = new[] { "Van", "Thi", "Minh", "Hoang", "Gia", "Duc", "Tuan", "Quoc", "Anh", "Hai" };
            var lastNames = new[] { "Hung", "Dung", "Nam", "An", "Binh", "Chinh", "Lam", "Linh", "Trang", "Huong", "Phong", "Phuc", "Quan", "Tuan", "Dat" };

            var random = new Random(42); // seed for reproducibility
            var students = new List<Student>();
            var usedEmails = new HashSet<string>();

            for (int i = 1; i <= 60; i++)
            {
                string fullName = $"{firstNames[random.Next(firstNames.Length)]} {middleNames[random.Next(middleNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
                string email = $"{fullName.Replace(" ", "").ToLower()}{i}@fpt.edu.vn";
                while (usedEmails.Contains(email))
                {
                    email = $"{fullName.Replace(" ", "").ToLower()}{i}{random.Next(100)}@fpt.edu.vn";
                }
                usedEmails.Add(email);

                students.Add(new Student
                {
                    FullName = fullName,
                    Email = email,
                    DateOfBirth = new DateTime(random.Next(2000, 2006), random.Next(1, 13), random.Next(1, 28))
                });
            }
            context.Students.AddRange(students);
            context.SaveChanges();
        }

        // 5. Seed Enrollments (Minimum 500)
        if (!context.Enrollments.Any())
        {
            var students = context.Students.ToList();
            var courses = context.Courses.ToList();
            var random = new Random(42);
            var enrollments = new List<Enrollment>();
            var uniquePairs = new HashSet<(int StudentId, int CourseId)>();
            var statuses = new[] { "Active", "Completed", "Dropped" };

            // Ensure we create exactly 500 unique enrollments
            while (enrollments.Count < 500)
            {
                var student = students[random.Next(students.Count)];
                var course = courses[random.Next(courses.Count)];

                if (!uniquePairs.Contains((student.StudentId, course.CourseId)))
                {
                    uniquePairs.Add((student.StudentId, course.CourseId));
                    enrollments.Add(new Enrollment
                    {
                        StudentId = student.StudentId,
                        CourseId = course.CourseId,
                        EnrollDate = DateTime.Now.AddDays(-random.Next(1, 100)),
                        Status = statuses[random.Next(statuses.Length)]
                    });
                }
            }
            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
        }
    }
}
