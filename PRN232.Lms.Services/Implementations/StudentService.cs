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

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _repository;
    private readonly IMapper _mapper;

    public StudentService(IRepository<Student> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<object>>> GetStudentsAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = _repository.GetQueryable();

        // 1. Search (FullName or Email)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => s.FullName.ToLower().Contains(searchLower) || s.Email.ToLower().Contains(searchLower));
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
        var mapped = _mapper.Map<List<StudentModel>>(items);

        // 6. Map to Response for API schema compatibility
        var responses = _mapper.Map<List<StudentResponse>>(mapped);

        // 7. Select Fields
        var resultList = responses.Select(m => QueryHelper.SelectFields(m, fields)).ToList();

        return new ApiResponse<List<object>>
        {
            Success = true,
            Message = "Students retrieved successfully",
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

    public async Task<ApiResponse<StudentModel>> GetStudentByIdAsync(int id)
    {
        var student = await _repository.GetQueryable()
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            return new ApiResponse<StudentModel>
            {
                Success = false,
                Message = $"Student with ID {id} not found",
                Errors = "Not Found"
            };
        }

        var response = _mapper.Map<StudentModel>(student);

        return new ApiResponse<StudentModel>
        {
            Success = true,
            Message = "Student retrieved successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<StudentModel>> CreateStudentAsync(StudentModel request)
    {
        // Check if email already exists
        var exists = await _repository.GetQueryable().AnyAsync(s => s.Email.ToLower() == request.Email.ToLower());
        if (exists)
        {
            return new ApiResponse<StudentModel>
            {
                Success = false,
                Message = $"Student with email {request.Email} already exists",
                Errors = "Bad Request"
            };
        }

        var student = _mapper.Map<Student>(request);

        await _repository.AddAsync(student);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<StudentModel>(student);

        return new ApiResponse<StudentModel>
        {
            Success = true,
            Message = "Student created successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<StudentModel>> UpdateStudentAsync(int id, StudentModel request)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null)
        {
            return new ApiResponse<StudentModel>
            {
                Success = false,
                Message = $"Student with ID {id} not found",
                Errors = "Not Found"
            };
        }

        // Check if email already exists for another student
        var exists = await _repository.GetQueryable().AnyAsync(s => s.Email.ToLower() == request.Email.ToLower() && s.StudentId != id);
        if (exists)
        {
            return new ApiResponse<StudentModel>
            {
                Success = false,
                Message = $"Student with email {request.Email} already exists",
                Errors = "Bad Request"
            };
        }

        _mapper.Map(request, student);

        _repository.Update(student);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<StudentModel>(student);

        return new ApiResponse<StudentModel>
        {
            Success = true,
            Message = "Student updated successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<bool>> DeleteStudentAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Student with ID {id} not found",
                Data = false,
                Errors = "Not Found"
            };
        }

        _repository.Delete(student);
        await _repository.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Student deleted successfully",
            Data = true
        };
    }
}
