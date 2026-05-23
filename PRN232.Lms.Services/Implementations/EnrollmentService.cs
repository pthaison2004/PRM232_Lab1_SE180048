using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PRN232.Lms.Repositories.Entities;
using PRN232.Lms.Repositories.Interfaces;
using PRN232.Lms.Services.Helpers;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.Lms.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IRepository<Enrollment> _repository;
    private readonly IMapper _mapper;

    public EnrollmentService(IRepository<Enrollment> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<object>>> GetEnrollmentsAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = _repository.GetQueryable();

        // 1. Search (Status)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(e => e.Status.ToLower().Contains(searchLower));
        }

        // 2. Expand/Include
        query = QueryHelper.ApplyExpansion(query, expand);

        // 3. Sort
        query = QueryHelper.ApplySorting(query, sort);

        // 4. Pagination
        var totalItems = await query.CountAsync();
        var pageNumber = page > 0 ? page : 1;
        var pageSize = size > 0 ? size : 10;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        // 5. Map to Business Model
        var mapped = _mapper.Map<List<EnrollmentModel>>(items);

        // 6. Map to Response for API schema compatibility
        var responses = _mapper.Map<List<EnrollmentResponse>>(mapped);

        // 7. Select Fields
        var resultList = responses.Select(m => QueryHelper.SelectFields(m, fields)).ToList();

        return new ApiResponse<List<object>>
        {
            Success = true,
            Message = "Enrollments retrieved successfully",
            Data = resultList,
            Pagination = new PaginatedResult
            {
                Page = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };
    }

    public async Task<ApiResponse<EnrollmentModel>> GetEnrollmentByIdAsync(int id)
    {
        var enrollment = await _repository.GetQueryable()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        if (enrollment == null)
        {
            return new ApiResponse<EnrollmentModel>
            {
                Success = false,
                Message = $"Enrollment with ID {id} not found",
                Errors = "Not Found"
            };
        }

        var response = _mapper.Map<EnrollmentModel>(enrollment);

        return new ApiResponse<EnrollmentModel>
        {
            Success = true,
            Message = "Enrollment retrieved successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<EnrollmentModel>> CreateEnrollmentAsync(EnrollmentModel request)
    {
        // Check for duplicate enrollment for same student and course
        var exists = await _repository.GetQueryable().AnyAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId);
        if (exists)
        {
            return new ApiResponse<EnrollmentModel>
            {
                Success = false,
                Message = $"Student with ID {request.StudentId} is already enrolled in Course {request.CourseId}",
                Errors = "Bad Request"
            };
        }

        var enrollment = _mapper.Map<Enrollment>(request);

        await _repository.AddAsync(enrollment);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<EnrollmentModel>(enrollment);

        return new ApiResponse<EnrollmentModel>
        {
            Success = true,
            Message = "Enrollment created successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<EnrollmentModel>> UpdateEnrollmentAsync(int id, EnrollmentModel request)
    {
        var enrollment = await _repository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return new ApiResponse<EnrollmentModel>
            {
                Success = false,
                Message = $"Enrollment with ID {id} not found",
                Errors = "Not Found"
            };
        }

        // Check for duplicate enrollment for another record
        var exists = await _repository.GetQueryable().AnyAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId && e.EnrollmentId != id);
        if (exists)
        {
            return new ApiResponse<EnrollmentModel>
            {
                Success = false,
                Message = $"Student with ID {request.StudentId} is already enrolled in Course {request.CourseId}",
                Errors = "Bad Request"
            };
        }

        _mapper.Map(request, enrollment);

        _repository.Update(enrollment);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<EnrollmentModel>(enrollment);

        return new ApiResponse<EnrollmentModel>
        {
            Success = true,
            Message = "Enrollment updated successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<bool>> DeleteEnrollmentAsync(int id)
    {
        var enrollment = await _repository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Enrollment with ID {id} not found",
                Data = false,
                Errors = "Not Found"
            };
        }

        _repository.Delete(enrollment);
        await _repository.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Enrollment deleted successfully",
            Data = true
        };
    }
}
