using CourseManager.API.DTOs;
using CourseManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChaptersController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChaptersController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetChaptersByCourseId(int courseId)
        {
            var result = await _chapterService.GetChaptersByCourseIdAsync(courseId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChapter(int id)
        {
            var result = await _chapterService.GetChapterByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter([FromBody] CreateChapterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));

            var result = await _chapterService.CreateChapterAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] UpdateChapterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));

            var result = await _chapterService.UpdateChapterAsync(id, dto);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var result = await _chapterService.DeleteChapterAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}