using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.Services.Interfaces;

public interface ISemesterService
{
    Task<ApiResponse<List<object>>> GetSemestersAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<ApiResponse<SemesterModel>> GetSemesterByIdAsync(int id);
    Task<ApiResponse<SemesterModel>> CreateSemesterAsync(SemesterModel request);
    Task<ApiResponse<SemesterModel>> UpdateSemesterAsync(int id, SemesterModel request);
    Task<ApiResponse<bool>> DeleteSemesterAsync(int id);
}
