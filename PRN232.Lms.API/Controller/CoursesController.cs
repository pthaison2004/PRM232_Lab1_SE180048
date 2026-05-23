using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.API.Controller;

[ApiController]
[Route("api/courses")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly IMapper _mapper;

    public CoursesController(ICourseService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách Course (hỗ trợ search, sort, paging, field selection, expand).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetCoursesAsync(search, sort, page, size, fields, expand);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một Course theo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourseById(int id)
    {
        var result = await _service.GetCourseByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ApiResponse<CourseResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<CourseResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<CourseResponse>(result.Data)
        });
    }

    /// <summary>
    /// Tạo mới một Course.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse([FromBody] CourseRequest request)
    {
        var dto = _mapper.Map<CourseModel>(request);
        var result = await _service.CreateCourseAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<CourseResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        var response = _mapper.Map<CourseResponse>(result.Data);
        return CreatedAtAction(nameof(GetCourseById), new { id = response.CourseId }, new ApiResponse<CourseResponse>
        {
            Success = true,
            Message = result.Message,
            Data = response
        });
    }

    /// <summary>
    /// Cập nhật một Course theo ID.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> UpdateCourse(int id, [FromBody] CourseRequest request)
    {
        var dto = _mapper.Map<CourseModel>(request);
        var result = await _service.UpdateCourseAsync(id, dto);
        if (!result.Success)
        {
            if (result.Errors?.ToString() == "Not Found")
            {
                return NotFound(new ApiResponse<CourseResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return BadRequest(new ApiResponse<CourseResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<CourseResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<CourseResponse>(result.Data)
        });
    }

    /// <summary>
    /// Xóa một Course theo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourse(int id)
    {
        var result = await _service.DeleteCourseAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
