using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManager.API.Data;
using CourseManager.API.DTOs;
using CourseManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.API.Services
{
    public class ChapterService : IChapterService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChapterService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<ChapterDto>>> GetChaptersByCourseIdAsync(int courseId)
        {
            var chapters = await _context.Chapters
                .Where(c => c.CourseId == courseId)
                .AsNoTracking()
                .ProjectTo<ChapterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ApiResponse<IEnumerable<ChapterDto>>.Ok(chapters, "Lấy danh sách chương thành công.");
        }

        public async Task<ApiResponse<ChapterDto>> GetChapterByIdAsync(int id)
        {
            var chapter = await _context.Chapters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            
            if (chapter == null)
            {
                return ApiResponse<ChapterDto>.Fail("Không tìm thấy chương.");
            }

            var dto = _mapper.Map<ChapterDto>(chapter);
            return ApiResponse<ChapterDto>.Ok(dto, "Lấy thông tin chương thành công.");
        }

        public async Task<ApiResponse<ChapterDto>> CreateChapterAsync(CreateChapterDto dto)
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == dto.CourseId);
            if (!courseExists)
                return ApiResponse<ChapterDto>.Fail("Khóa học không tồn tại.");

            var chapter = _mapper.Map<Chapter>(dto);
            
            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<ChapterDto>(chapter);
            return ApiResponse<ChapterDto>.Ok(createdDto, "Tạo chương thành công.");
        }

        public async Task<ApiResponse<ChapterDto>> UpdateChapterAsync(int id, UpdateChapterDto dto)
        {
            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null)
                return ApiResponse<ChapterDto>.Fail("Không tìm thấy chương.");

            _mapper.Map(dto, chapter);
            await _context.SaveChangesAsync();

            var updatedDto = _mapper.Map<ChapterDto>(chapter);
            return ApiResponse<ChapterDto>.Ok(updatedDto, "Cập nhật chương thành công.");
        }

        public async Task<ApiResponse<bool>> DeleteChapterAsync(int id)
        {
            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id);
            if (chapter == null)
                return ApiResponse<bool>.Fail("Không tìm thấy chương.");

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Xóa chương thành công.");
        }
    }
}