namespace miniReddit.Models
{
    public class Post(string userId, string categoryId, string content, string title, string? img)
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = userId;
        public string CategoryId { get; set; } = categoryId;
        public string TextContent { get; set; } = content;
        public string Title { get; set; } = title;
        public string? ImgPath { get; set; } = img;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
