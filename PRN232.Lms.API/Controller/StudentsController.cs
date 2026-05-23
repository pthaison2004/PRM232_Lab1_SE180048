using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.API.Controller;

[ApiController]
[Route("api/students")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IMapper _mapper;

    public StudentsController(IStudentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách Student (hỗ trợ search, sort, paging, field selection, expand).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetStudents(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetStudentsAsync(search, sort, page, size, fields, expand);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một Student theo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> GetStudentById(int id)
    {
        var result = await _service.GetStudentByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<StudentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<StudentResponse>(result.Data)
        });
    }

    /// <summary>
    /// Tạo mới một Student.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> CreateStudent([FromBody] StudentRequest request)
    {
        var dto = _mapper.Map<StudentModel>(request);
        var result = await _service.CreateStudentAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        var response = _mapper.Map<StudentResponse>(result.Data);
        return CreatedAtAction(nameof(GetStudentById), new { id = response.StudentId }, new ApiResponse<StudentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = response
        });
    }

    /// <summary>
    /// Cập nhật một Student theo ID.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<StudentResponse>>> UpdateStudent(int id, [FromBody] StudentRequest request)
    {
        var dto = _mapper.Map<StudentModel>(request);
        var result = await _service.UpdateStudentAsync(id, dto);
        if (!result.Success)
        {
            if (result.Errors?.ToString() == "Not Found")
            {
                return NotFound(new ApiResponse<StudentResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return BadRequest(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<StudentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<StudentResponse>(result.Data)
        });
    }

    /// <summary>
    /// Xóa một Student theo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteStudent(int id)
    {
        var result = await _service.DeleteStudentAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
