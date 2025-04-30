namespace miniReddit.Models
{
    public class Post(User user, Category category, string content, string title)
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = user.Id;
        public string CategoryId { get; set; } = category.Id;
        public string Content { get; set; } = content;
        public string Title { get; set; } = title;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
