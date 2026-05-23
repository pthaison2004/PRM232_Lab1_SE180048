using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.API.Controller;

[ApiController]
[Route("api/subjects")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;
    private readonly IMapper _mapper;

    public SubjectsController(ISubjectService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách Subject (hỗ trợ search, sort, paging, field selection, expand).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetSubjects(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetSubjectsAsync(search, sort, page, size, fields, expand);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một Subject theo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> GetSubjectById(int id)
    {
        var result = await _service.GetSubjectByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ApiResponse<SubjectResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<SubjectResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<SubjectResponse>(result.Data)
        });
    }

    /// <summary>
    /// Tạo mới một Subject.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> CreateSubject([FromBody] SubjectRequest request)
    {
        var dto = _mapper.Map<SubjectModel>(request);
        var result = await _service.CreateSubjectAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<SubjectResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        var response = _mapper.Map<SubjectResponse>(result.Data);
        return CreatedAtAction(nameof(GetSubjectById), new { id = response.SubjectId }, new ApiResponse<SubjectResponse>
        {
            Success = true,
            Message = result.Message,
            Data = response
        });
    }

    /// <summary>
    /// Cập nhật một Subject theo ID.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SubjectResponse>>> UpdateSubject(int id, [FromBody] SubjectRequest request)
    {
        var dto = _mapper.Map<SubjectModel>(request);
        var result = await _service.UpdateSubjectAsync(id, dto);
        if (!result.Success)
        {
            if (result.Errors?.ToString() == "Not Found")
            {
                return NotFound(new ApiResponse<SubjectResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return BadRequest(new ApiResponse<SubjectResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<SubjectResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<SubjectResponse>(result.Data)
        });
    }

    /// <summary>
    /// Xóa một Subject theo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSubject(int id)
    {
        var result = await _service.DeleteSubjectAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
