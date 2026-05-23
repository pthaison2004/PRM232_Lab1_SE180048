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

public class SubjectService : ISubjectService
{
    private readonly IRepository<Subject> _repository;
    private readonly IMapper _mapper;

    public SubjectService(IRepository<Subject> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<object>>> GetSubjectsAsync(string? search, string? sort, int page, int size, string? fields, string? expand)
    {
        var query = _repository.GetQueryable();

        // 1. Search (SubjectCode or SubjectName)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(s => s.SubjectCode.ToLower().Contains(searchLower) || s.SubjectName.ToLower().Contains(searchLower));
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
        var mapped = _mapper.Map<List<SubjectModel>>(items);

        // 6. Map to Response for API schema compatibility
        var responses = _mapper.Map<List<SubjectResponse>>(mapped);

        // 7. Select Fields
        var resultList = responses.Select(m => QueryHelper.SelectFields(m, fields)).ToList();

        return new ApiResponse<List<object>>
        {
            Success = true,
            Message = "Subjects retrieved successfully",
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

    public async Task<ApiResponse<SubjectModel>> GetSubjectByIdAsync(int id)
    {
        var subject = await _repository.GetByIdAsync(id);
        if (subject == null)
        {
            return new ApiResponse<SubjectModel>
            {
                Success = false,
                Message = $"Subject with ID {id} not found",
                Errors = "Not Found"
            };
        }

        var response = _mapper.Map<SubjectModel>(subject);

        return new ApiResponse<SubjectModel>
        {
            Success = true,
            Message = "Subject retrieved successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<SubjectModel>> CreateSubjectAsync(SubjectModel request)
    {
        // Check for unique subject code
        var exists = await _repository.GetQueryable().AnyAsync(s => s.SubjectCode.ToLower() == request.SubjectCode.ToLower());
        if (exists)
        {
            return new ApiResponse<SubjectModel>
            {
                Success = false,
                Message = $"Subject with code {request.SubjectCode} already exists",
                Errors = "Bad Request"
            };
        }

        var subject = _mapper.Map<Subject>(request);

        await _repository.AddAsync(subject);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<SubjectModel>(subject);

        return new ApiResponse<SubjectModel>
        {
            Success = true,
            Message = "Subject created successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<SubjectModel>> UpdateSubjectAsync(int id, SubjectModel request)
    {
        var subject = await _repository.GetByIdAsync(id);
        if (subject == null)
        {
            return new ApiResponse<SubjectModel>
            {
                Success = false,
                Message = $"Subject with ID {id} not found",
                Errors = "Not Found"
            };
        }

        // Check for unique subject code for other records
        var exists = await _repository.GetQueryable().AnyAsync(s => s.SubjectCode.ToLower() == request.SubjectCode.ToLower() && s.SubjectId != id);
        if (exists)
        {
            return new ApiResponse<SubjectModel>
            {
                Success = false,
                Message = $"Subject with code {request.SubjectCode} already exists",
                Errors = "Bad Request"
            };
        }

        _mapper.Map(request, subject);

        _repository.Update(subject);
        await _repository.SaveChangesAsync();

        var response = _mapper.Map<SubjectModel>(subject);

        return new ApiResponse<SubjectModel>
        {
            Success = true,
            Message = "Subject updated successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<bool>> DeleteSubjectAsync(int id)
    {
        var subject = await _repository.GetByIdAsync(id);
        if (subject == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Subject with ID {id} not found",
                Data = false,
                Errors = "Not Found"
            };
        }

        _repository.Delete(subject);
        await _repository.SaveChangesAsync();

        return new ApiResponse<bool>
        {
            Success = true,
            Message = "Subject deleted successfully",
            Data = true
        };
    }
}
