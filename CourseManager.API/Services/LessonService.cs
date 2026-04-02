using AutoMapper;
using AutoMapper.QueryableExtensions;
using CourseManager.API.Data;
using CourseManager.API.DTOs;
using CourseManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.API.Services
{
    public class LessonService : ILessonService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LessonService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<LessonDto>>> GetLessonsByChapterIdAsync(int chapterId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.ChapterId == chapterId)
                .AsNoTracking()
                .ProjectTo<LessonDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return ApiResponse<IEnumerable<LessonDto>>.Ok(lessons, "Lấy danh sách bài học thành công.");
        }

        public async Task<ApiResponse<LessonDto>> GetLessonByIdAsync(int id)
        {
            var lesson = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            
            if (lesson == null)
                return ApiResponse<LessonDto>.Fail("Không tìm thấy bài học.");

            var dto = _mapper.Map<LessonDto>(lesson);
            return ApiResponse<LessonDto>.Ok(dto, "Lấy thông tin bài học thành công.");
        }

        public async Task<ApiResponse<LessonDto>> CreateLessonAsync(CreateLessonDto dto)
        {
            var chapterExists = await _context.Chapters.AnyAsync(c => c.Id == dto.ChapterId);
            if (!chapterExists)
                return ApiResponse<LessonDto>.Fail("Chương học không tồn tại.");

            var lesson = _mapper.Map<Lesson>(dto);
            
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<LessonDto>(lesson);
            return ApiResponse<LessonDto>.Ok(createdDto, "Tạo bài học thành công.");
        }

        public async Task<ApiResponse<LessonDto>> UpdateLessonAsync(int id, UpdateLessonDto dto)
        {
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
                return ApiResponse<LessonDto>.Fail("Không tìm thấy bài học.");

            _mapper.Map(dto, lesson);
            await _context.SaveChangesAsync();

            var updatedDto = _mapper.Map<LessonDto>(lesson);
            return ApiResponse<LessonDto>.Ok(updatedDto, "Cập nhật bài học thành công.");
        }

        public async Task<ApiResponse<bool>> DeleteLessonAsync(int id)
        {
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
                return ApiResponse<bool>.Fail("Không tìm thấy bài học.");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Xóa bài học thành công.");
        }
    }
}