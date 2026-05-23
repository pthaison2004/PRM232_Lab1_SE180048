using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.Services.Interfaces;

public interface IEnrollmentService
{
    Task<ApiResponse<List<object>>> GetEnrollmentsAsync(string? search, string? sort, int page, int size, string? fields, string? expand);
    Task<ApiResponse<EnrollmentModel>> GetEnrollmentByIdAsync(int id);
    Task<ApiResponse<EnrollmentModel>> CreateEnrollmentAsync(EnrollmentModel request);
    Task<ApiResponse<EnrollmentModel>> UpdateEnrollmentAsync(int id, EnrollmentModel request);
    Task<ApiResponse<bool>> DeleteEnrollmentAsync(int id);
}
