using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.Services.Interfaces;

public interface ISubjectService
{
    Task<ApiResponse<List<object>>> GetSubjectsAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<ApiResponse<SubjectModel>> GetSubjectByIdAsync(int id);
    Task<ApiResponse<SubjectModel>> CreateSubjectAsync(SubjectModel request);
    Task<ApiResponse<SubjectModel>> UpdateSubjectAsync(int id, SubjectModel request);
    Task<ApiResponse<bool>> DeleteSubjectAsync(int id);
}
