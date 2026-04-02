using CourseManager.API.DTOs;

namespace CourseManager.API.Services
{
    public interface ILessonService
    {
        Task<ApiResponse<IEnumerable<LessonDto>>> GetLessonsByChapterIdAsync(int chapterId);
        Task<ApiResponse<LessonDto>> GetLessonByIdAsync(int id);
        Task<ApiResponse<LessonDto>> CreateLessonAsync(CreateLessonDto dto);
        Task<ApiResponse<LessonDto>> UpdateLessonAsync(int id, UpdateLessonDto dto);
        Task<ApiResponse<bool>> DeleteLessonAsync(int id);
    }
}