namespace CourseManager.API.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int ChapterId { get; set; }
        public Chapter? Chapter { get; set; }
    }
}