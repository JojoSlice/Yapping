using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace miniReddit.Models
{
    public class Comment(User user, Post post, string content)
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("userid")]
        public string UserId { get; set; } = user.Id;
        [JsonPropertyName("postid")]
        public string PostId { get; set; } = post.Id;
        [JsonPropertyName("text")]
        public string Text { get; set; } = content;
        [JsonPropertyName("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
