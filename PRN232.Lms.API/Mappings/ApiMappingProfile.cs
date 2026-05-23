using AutoMapper;
using PRN232.Lms.Services.Models;

namespace PRN232.Lms.API.Mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // Student Mappings
        CreateMap<StudentRequest, StudentModel>();
        CreateMap<StudentModel, StudentResponse>();

        // Course Mappings
        CreateMap<CourseRequest, CourseModel>();
        CreateMap<CourseModel, CourseResponse>();

        // Enrollment Mappings
        CreateMap<EnrollmentRequest, EnrollmentModel>();
        CreateMap<EnrollmentModel, EnrollmentResponse>();

        // Semester Mappings
        CreateMap<SemesterRequest, SemesterModel>();
        CreateMap<SemesterModel, SemesterResponse>();

        // Subject Mappings
        CreateMap<SubjectRequest, SubjectModel>();
        CreateMap<SubjectModel, SubjectResponse>();
    }
}
