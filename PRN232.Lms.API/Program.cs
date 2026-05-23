using Microsoft.EntityFrameworkCore;
using PRN232.Lms.Repositories.Data;
using PRN232.Lms.Repositories.Interfaces;
using PRN232.Lms.Repositories.Implementations;
using PRN232.Lms.Services.Interfaces;
using PRN232.Lms.Services.Implementations;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Controller: Chống vòng lặp vô hạn (Expand) và tự động ẩn trường null (Field Selection)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. Cấu hình Kết nối SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=localhost;Database=LmsDb;User Id=sa;Password=12345;TrustServerCertificate=True;";
    options.UseSqlServer(connectionString);
});

// 3. Đăng ký Dependency Injection cho toàn hệ thống
// --- AutoMapper ---
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<PRN232.Lms.Services.Mappings.MappingProfile>();
    cfg.AddProfile<PRN232.Lms.API.Mappings.ApiMappingProfile>();
});

// --- Repositories ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// --- Services ---
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// 4. Cấu hình tích hợp giao diện Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LMS API v1");
        c.RoutePrefix = string.Empty; // Đặt thẳng Swagger làm trang chủ mặc định khi chạy
    });
}

// 5.1. Global Exception Handler - Bắt mọi lỗi không lường trước và trả về ApiResponse 500
app.UseMiddleware<PRN232.Lms.API.Middlewares.GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 6. Cơ chế tự động Seeding dữ liệu mẫu khi hệ thống khởi tạo (Rất tối ưu khi chạy Docker)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<AppDbContext>();

    int retries = 5;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database and seed data...");
            // Thực hiện gọi class Seeder của bạn
            DbSeeder.Seed(context);
            logger.LogInformation("Database seeded successfully.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning(ex, "Database seeding failed. Retrying in 5 seconds... ({Retries} attempts left)", retries);
            if (retries == 0)
            {
                logger.LogError(ex, "An error occurred while seeding the database after multiple attempts.");
            }
            else
            {
                Thread.Sleep(5000);
            }
        }
    }
}

app.Run();