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

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _repository;
    private readonly IMapper _mapper;

    public CourseService(IRepository<Course> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<object>>> GetCoursesAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = _repository.GetQueryable();

        // 1. Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(c => c.CourseName.ToLower().Contains(searchLower));
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
        var mapped = _mapper.Map<List<CourseModel>>(items);

        // 6. Map to Response for API schema compatibility
        var responses = _mapper.Map<List<CourseResponse>>(mapped);

        // 7. Select Fields
        var resultList = responses.Select(m => QueryHelper.SelectFields(m, fields)).ToList();

        return new ApiResponse<List<object>>
        {
            Success = true,
            Message = "Courses retrieved successfully",
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

    public async Task<ApiResponse<CourseModel>> GetCourseByIdAsync(int id)
    {
        var course = await _repository.GetQueryable()
            .Include(c => c.Semester)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.CourseId == id);

        if (course == null)
        {
            return new ApiResponse<CourseModel>
            {
                Success = false,
                Message = $"Course with ID {id} not found",
                Errors = "Not Found"
            };
        }

        var response = _mapper.Map<CourseModel>(course);

        return new ApiResponse<CourseModel>
        {
            Success = true,
            Message = "Course retrieved successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<CourseModel>> CreateCourseAsync(CourseModel request)
    {
        var course = _mapper.Map<Course>(request);

        await _repository.AddAsync(course);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<CourseModel>(course);

        return new ApiResponse<CourseModel>
        {
            Success = true,
            Message = "Course created successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<CourseModel>> UpdateCourseAsync(int id, CourseModel request)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
        {
            return new ApiResponse<CourseModel>
            {
                Success = false,
                Message = $"Course with ID {id} not found",
                Errors = "Not Found"
            };
        }

        _mapper.Map(request, course);

        _repository.Update(course);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<CourseModel>(course);

        return new ApiResponse<CourseModel>
        {
            Success = true,
            Message = "Course updated successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<bool>> DeleteCourseAsync(int id)
    {
        var course = await _repository.GetByIdAsync(id);
        if (course == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Course with ID {id} not found",
                Data = false,
                Errors = "Not Found"
            };
        }

        _repository.Delete(course);
        await _repository.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Course deleted successfully",
            Data = true
        };
    }
}
