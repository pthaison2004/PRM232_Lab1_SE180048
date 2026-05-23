using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.Services.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<List<object>>> GetStudentsAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<ApiResponse<StudentModel>> GetStudentByIdAsync(int id);
    Task<ApiResponse<StudentModel>> CreateStudentAsync(StudentModel request);
    Task<ApiResponse<StudentModel>> UpdateStudentAsync(int id, StudentModel request);
    Task<ApiResponse<bool>> DeleteStudentAsync(int id);
}
