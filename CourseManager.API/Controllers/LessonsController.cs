using CourseManager.API.DTOs;
using CourseManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<IActionResult> GetLessonsByChapterId(int chapterId)
        {
            var result = await _lessonService.GetLessonsByChapterIdAsync(chapterId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesson(int id)
        {
            var result = await _lessonService.GetLessonByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));

            var result = await _lessonService.CreateLessonAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));

            var result = await _lessonService.UpdateLessonAsync(id, dto);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var result = await _lessonService.DeleteLessonAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}