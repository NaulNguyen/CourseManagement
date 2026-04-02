using CourseManager.API.Data;
using CourseManager.API.DTOs;
using CourseManager.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace CourseManager.API.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;

        // Key để theo dõi danh sách các cache đã tạo, thực tế có thể dùng IDistributedCache hoặc CancellationTokenSource
        private const string MemoryCacheKeys = "CourseCacheKeys"; 

        public CourseService(AppDbContext context, IMemoryCache cache, IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<CourseDto>>> GetCoursesAsync(string searchTerm, string sortBy, int page, int pageSize)
        {
            string cacheKey = $"courses_{searchTerm}_{sortBy}_{page}_{pageSize}";
            
            if (_cache.TryGetValue(cacheKey, out PagedResult<CourseDto>? cachedData))
            {
                return ApiResponse<PagedResult<CourseDto>>.Ok(cachedData!, "Lấy danh sách khóa học từ cache thành công.");
            }

            var query = _context.Courses.AsNoTracking().AsQueryable();

            // 1. Search không phân biệt hoa thường (ToLower())
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(c => c.Title.ToLower().Contains(lowerSearchTerm));
            }

            // 2. Sorting
            query = sortBy.ToLower() switch
            {
                "price_desc" => query.OrderByDescending(c => c.Price),
                "price_asc" => query.OrderBy(c => c.Price),
                _ => query.OrderBy(c => c.Title)
            };

            var totalItems = await query.CountAsync();

            // 3. Sử dụng AutoMapper ProjectTo để map Entity -> DTO ngay trong câu query
            var courses = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var result = new PagedResult<CourseDto>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Data = courses
            };

            // Lưu dữ liệu vào cache
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            TrackCacheKey(cacheKey);

            return ApiResponse<PagedResult<CourseDto>>.Ok(result, "Lấy danh sách khóa học thành công.");
        }

        public async Task<ApiResponse<CourseDto>> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ApiResponse<CourseDto>.Fail("Không tìm thấy khóa học.");
            }

            var courseDto = _mapper.Map<CourseDto>(course);
            return ApiResponse<CourseDto>.Ok(courseDto, "Lấy thông tin khóa học thành công.");
        }

        public async Task<ApiResponse<CourseDetailDto>> GetCourseDetailByIdAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Chapters!)
                    .ThenInclude(ch => ch.Lessons!)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return ApiResponse<CourseDetailDto>.Fail("Không tìm thấy khóa học.");

            var dto = _mapper.Map<CourseDetailDto>(course);
            return ApiResponse<CourseDetailDto>.Ok(dto, "Lấy chi tiết khóa học và lộ trình học thành công.");
        }

        public async Task<ApiResponse<bool>> EnrollCourseAsync(string userId, int courseId)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
                return ApiResponse<bool>.Fail("Khóa học không tồn tại.");

            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
            if (alreadyEnrolled)
                return ApiResponse<bool>.Fail("Bạn đã ghi danh khóa học này rồi.");

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Ghi danh khóa học thành công.");
        }

        public async Task<ApiResponse<IEnumerable<CourseDto>>> GetMyEnrolledCoursesAsync(string userId)
        {
            var courses = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ProjectTo<CourseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ApiResponse<IEnumerable<CourseDto>>.Ok(courses, "Lấy danh sách khóa học đang theo học thành công.");
        }

        public async Task<ApiResponse<CourseDto>> CreateCourseAsync(CreateCourseDto dto)
        {
            // Sử dụng AutoMapper để map DTO -> Entity
            var course = _mapper.Map<Course>(dto);

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // 4. Clear cache sau khi tạo course
            ClearCourseCaches();

            // Sử dụng AutoMapper để map ngược Entity -> DTO trả về
            var createdDto = _mapper.Map<CourseDto>(course);

            return ApiResponse<CourseDto>.Ok(createdDto, "Tạo khóa học thành công.");
        }

        public async Task<ApiResponse<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto dto)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ApiResponse<CourseDto>.Fail("Không tìm thấy khóa học.");
            }

            // Map đè giá trị từ DTO vào Entity có sẵn
            _mapper.Map(dto, course);

            await _context.SaveChangesAsync();
            ClearCourseCaches();

            var updatedDto = _mapper.Map<CourseDto>(course);
            return ApiResponse<CourseDto>.Ok(updatedDto, "Cập nhật khóa học thành công.");
        }

        public async Task<ApiResponse<bool>> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ApiResponse<bool>.Fail("Không tìm thấy khóa học.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            ClearCourseCaches();

            return ApiResponse<bool>.Ok(true, "Xóa khóa học thành công.");
        }

        private void TrackCacheKey(string key)
        {
            var keys = _cache.Get<List<string>>(MemoryCacheKeys) ?? new List<string>();
            if (!keys.Contains(key))
            {
                keys.Add(key);
                _cache.Set(MemoryCacheKeys, keys);
            }
        }

        private void ClearCourseCaches()
        {
            var keys = _cache.Get<List<string>>(MemoryCacheKeys) ?? new List<string>();
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
            _cache.Remove(MemoryCacheKeys);
        }
    }
}