using System.Security.Claims;
using CourseManager.API.DTOs;
using CourseManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 1. Added Authorization
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourses(
            [FromQuery] string searchTerm = "",
            [FromQuery] string sortBy = "price_asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _courseService.GetCoursesAsync(searchTerm, sortBy, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourse(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);
            if (!result.Success) return NotFound(result);

            return Ok(result);
        }

        [HttpGet("{id}/detail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseDetail(int id)
        {
            var result = await _courseService.GetCourseDetailByIdAsync(id);
            if (!result.Success) return NotFound(result);

            return Ok(result);
        }

        [HttpPost("{id}/enroll")]
        public async Task<IActionResult> EnrollCourse(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _courseService.EnrollCourseAsync(userId, id);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("my-enrolled")]
        public async Task<IActionResult> GetMyEnrolledCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _courseService.GetMyEnrolledCoursesAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            // (Validation can be caught via middleware or automatic validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));
            }

            var result = await _courseService.CreateCourseAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Dữ liệu đầu vào không hợp lệ."));
            }

            var result = await _courseService.UpdateCourseAsync(id, dto);
            if (!result.Success) return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (!result.Success) return NotFound(result);

            return Ok(result);
        }
    }
}