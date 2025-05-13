using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace miniReddit.Models
{
    public class Post(string userId, string categoryId, string content, string title, string? img)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("userid")]
        public string UserId { get; set; } = userId;
        [JsonPropertyName("categoryid")]
        public string CategoryId { get; set; } = categoryId;
        [JsonPropertyName("text")]
        public string Text { get; set; } = content;
        [JsonPropertyName("title")]
        public string Title { get; set; } = title;
        [JsonPropertyName("imgpath")]
        public string? ImgPath { get; set; } = img;
        [JsonPropertyName("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
