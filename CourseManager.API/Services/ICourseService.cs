using CourseManager.API.DTOs;

namespace CourseManager.API.Services
{
    public interface ICourseService
    {
        Task<ApiResponse<PagedResult<CourseDto>>> GetCoursesAsync(string searchTerm, string sortBy, int page, int pageSize);
        Task<ApiResponse<CourseDto>> GetCourseByIdAsync(int id);
        Task<ApiResponse<CourseDetailDto>> GetCourseDetailByIdAsync(int courseId);
        Task<ApiResponse<CourseDto>> CreateCourseAsync(CreateCourseDto dto);
        Task<ApiResponse<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto dto);
        Task<ApiResponse<bool>> DeleteCourseAsync(int id);
        Task<ApiResponse<bool>> EnrollCourseAsync(string userId, int courseId);
        Task<ApiResponse<IEnumerable<CourseDto>>> GetMyEnrolledCoursesAsync(string userId);
    }
}