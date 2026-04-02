namespace CourseManager.API.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    // 1. DTO chuẩn để trả về thay vì Entity
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class CourseDetailDto : CourseDto
    {
        public List<ChapterDetailDto> Chapters { get; set; } = new List<ChapterDetailDto>();
    }

    // 2. Định dạng Response DTO chuẩn
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Thành công") 
            => new ApiResponse<T> { Success = true, Message = message, Data = data };
        
        public static ApiResponse<T> Fail(string message) 
            => new ApiResponse<T> { Success = false, Message = message, Data = default };
    }

    // 3. Response DTO cho phân trang
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}