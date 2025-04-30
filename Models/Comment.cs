namespace miniReddit.Models
{
    public class Comment(User user, Post post, string content)
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = user.Id;
        public string PostId { get; set; } = post.Id;
        public string Content { get; set; } = content;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
