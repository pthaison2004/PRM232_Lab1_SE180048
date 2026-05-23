using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.Lms.API.Controller;

[ApiController]
[Route("api/enrollments")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;
    private readonly IMapper _mapper;

    public EnrollmentsController(IEnrollmentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách Enrollment (hỗ trợ search, sort, paging, field selection, expand).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetEnrollments(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? fields = null,
        [FromQuery] string? expand = null)
    {
        var result = await _service.GetEnrollmentsAsync(search, sort, page, size, fields, expand);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết một Enrollment theo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> GetEnrollmentById(int id)
    {
        var result = await _service.GetEnrollmentByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ApiResponse<EnrollmentResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<EnrollmentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<EnrollmentResponse>(result.Data)
        });
    }

    /// <summary>
    /// Tạo mới một Enrollment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> CreateEnrollment([FromBody] EnrollmentRequest request)
    {
        var dto = _mapper.Map<EnrollmentModel>(request);
        var result = await _service.CreateEnrollmentAsync(dto);
        if (!result.Success)
        {
            return BadRequest(new ApiResponse<EnrollmentResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        var response = _mapper.Map<EnrollmentResponse>(result.Data);
        return CreatedAtAction(nameof(GetEnrollmentById), new { id = response.EnrollmentId }, new ApiResponse<EnrollmentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = response
        });
    }

    /// <summary>
    /// Cập nhật một Enrollment theo ID.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> UpdateEnrollment(int id, [FromBody] EnrollmentRequest request)
    {
        var dto = _mapper.Map<EnrollmentModel>(request);
        var result = await _service.UpdateEnrollmentAsync(id, dto);
        if (!result.Success)
        {
            if (result.Errors?.ToString() == "Not Found")
            {
                return NotFound(new ApiResponse<EnrollmentResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return BadRequest(new ApiResponse<EnrollmentResponse>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors
            });
        }
        return Ok(new ApiResponse<EnrollmentResponse>
        {
            Success = true,
            Message = result.Message,
            Data = _mapper.Map<EnrollmentResponse>(result.Data)
        });
    }

    /// <summary>
    /// Xóa một Enrollment theo ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEnrollment(int id)
    {
        var result = await _service.DeleteEnrollmentAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
