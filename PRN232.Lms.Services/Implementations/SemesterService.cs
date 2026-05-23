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

public class SemesterService : ISemesterService
{
    private readonly IRepository<Semester> _repository;
    private readonly IMapper _mapper;

    public SemesterService(IRepository<Semester> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<object>>> GetSemestersAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = _repository.GetQueryable();

        // 1. Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => s.SemesterName.ToLower().Contains(searchLower));
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
        var mapped = _mapper.Map<List<SemesterModel>>(items);

        // 6. Map to Response for API schema compatibility
        var responses = _mapper.Map<List<SemesterResponse>>(mapped);

        // 7. Select Fields
        var resultList = responses.Select(m => QueryHelper.SelectFields(m, fields)).ToList();

        return new ApiResponse<List<object>>
        {
            Success = true,
            Message = "Semesters retrieved successfully",
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

    public async Task<ApiResponse<SemesterModel>> GetSemesterByIdAsync(int id)
    {
        var semester = await _repository.GetQueryable()
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.SemesterId == id);

        if (semester == null)
        {
            return new ApiResponse<SemesterModel>
            {
                Success = false,
                Message = $"Semester with ID {id} not found",
                Errors = "Not Found"
            };
        }

        var response = _mapper.Map<SemesterModel>(semester);

        return new ApiResponse<SemesterModel>
        {
            Success = true,
            Message = "Semester retrieved successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<SemesterModel>> CreateSemesterAsync(SemesterModel request)
    {
        var semester = _mapper.Map<Semester>(request);

        await _repository.AddAsync(semester);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<SemesterModel>(semester);

        return new ApiResponse<SemesterModel>
        {
            Success = true,
            Message = "Semester created successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<SemesterModel>> UpdateSemesterAsync(int id, SemesterModel request)
    {
        var semester = await _repository.GetByIdAsync(id);
        if (semester == null)
        {
            return new ApiResponse<SemesterModel>
            {
                Success = false,
                Message = $"Semester with ID {id} not found",
                Errors = "Not Found"
            };
        }

        _mapper.Map(request, semester);

        _repository.Update(semester);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<SemesterModel>(semester);

        return new ApiResponse<SemesterModel>
        {
            Success = true,
            Message = "Semester updated successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<bool>> DeleteSemesterAsync(int id)
    {
        var semester = await _repository.GetByIdAsync(id);
        if (semester == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Semester with ID {id} not found",
                Data = false,
                Errors = "Not Found"
            };
        }

        _repository.Delete(semester);
        await _repository.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Semester deleted successfully",
            Data = true
        };
    }
}
