using AutoMapper;
using CourseManager.API.DTOs;
using CourseManager.API.Models;

namespace CourseManager.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map từ Course Entity -> CourseDto
            CreateMap<Course, CourseDto>();
            CreateMap<Course, CourseDetailDto>();
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // Chapter
            CreateMap<Chapter, ChapterDto>();
            CreateMap<Chapter, ChapterDetailDto>();
            CreateMap<CreateChapterDto, Chapter>();
            CreateMap<UpdateChapterDto, Chapter>();

            // Lesson
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>();
            CreateMap<UpdateLessonDto, Lesson>();
        }
    }
}