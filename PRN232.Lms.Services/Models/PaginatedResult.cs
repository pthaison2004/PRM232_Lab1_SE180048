namespace PRN232.Lms.Services.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = "Request processed successfully";
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public PaginatedResult? Pagination { get; set; }
}

public class PaginatedResult
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
