using AutoMapper;
using PRN232.Lms.Repositories.Entities;
using PRN232.Lms.Services.Models;

namespace PRN232.Lms.Services.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentModel>().ReverseMap();
        CreateMap<Course, CourseModel>().ReverseMap();
        CreateMap<Enrollment, EnrollmentModel>().ReverseMap();
        CreateMap<Semester, SemesterModel>().ReverseMap();
        CreateMap<Subject, SubjectModel>().ReverseMap();
    }
}
