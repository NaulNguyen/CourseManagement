using CourseManager.API.DTOs;

namespace CourseManager.API.Services
{
    public interface IChapterService
    {
        Task<ApiResponse<IEnumerable<ChapterDto>>> GetChaptersByCourseIdAsync(int courseId);
        Task<ApiResponse<ChapterDto>> GetChapterByIdAsync(int id);
        Task<ApiResponse<ChapterDto>> CreateChapterAsync(CreateChapterDto dto);
        Task<ApiResponse<ChapterDto>> UpdateChapterAsync(int id, UpdateChapterDto dto);
        Task<ApiResponse<bool>> DeleteChapterAsync(int id);
    }
}