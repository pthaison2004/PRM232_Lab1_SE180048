using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.Services.Interfaces;

public interface ICourseService
{
    Task<ApiResponse<List<object>>> GetCoursesAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<ApiResponse<CourseModel>> GetCourseByIdAsync(int id);
    Task<ApiResponse<CourseModel>> CreateCourseAsync(CourseModel request);
    Task<ApiResponse<CourseModel>> UpdateCourseAsync(int id, CourseModel request);
    Task<ApiResponse<bool>> DeleteCourseAsync(int id);
}
