using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.API.Controller;

[ApiController]
[Route("api/semesters")]
[Produces("application/json")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;
    private readonly IMapper _mapper;

    public SemestersController(ISemesterService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách Semester (hỗ trợ search, sort, paging, field selection, expand).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetSemesters(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetSemestersAsync(search, sort, page, size, fields, expand);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một Semester theo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> GetSemesterById(int id)
    {
        var result = await _service.GetSemesterByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ApiResponse<SemesterResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<SemesterResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<SemesterResponse>(result.Data)
        });
    }

    /// <summary>
    /// Tạo mới một Semester.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> CreateSemester([FromBody] SemesterRequest request)
    {
        var dto = _mapper.Map<SemesterModel>(request);
        var result = await _service.CreateSemesterAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<SemesterResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        var response = _mapper.Map<SemesterResponse>(result.Data);
        return CreatedAtAction(nameof(GetSemesterById), new { id = response.SemesterId }, new ApiResponse<SemesterResponse>
        {
            Success = true,
            Message = result.Message,
            Data = response
        });
    }

    /// <summary>
    /// Cập nhật một Semester theo ID.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<SemesterResponse>>> UpdateSemester(int id, [FromBody] SemesterRequest request)
    {
        var dto = _mapper.Map<SemesterModel>(request);
        var result = await _service.UpdateSemesterAsync(id, dto);
        if (!result.Success)
        {
            if (result.Errors?.ToString() == "Not Found")
            {
                return NotFound(new ApiResponse<SemesterResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return BadRequest(new ApiResponse<SemesterResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<SemesterResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<SemesterResponse>(result.Data)
        });
    }

    /// <summary>
    /// Xóa một Semester theo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSemester(int id)
    {
        var result = await _service.DeleteSemesterAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
