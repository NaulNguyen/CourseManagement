namespace CourseManager.API.DTOs
{
    public class ChapterDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }

    public class ChapterDetailDto : ChapterDto
    {
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    }

    public class CreateChapterDto
    {
        public string Title { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }

    public class UpdateChapterDto
    {
        public string Title { get; set; } = string.Empty;
    }
}