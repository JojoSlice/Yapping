using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace miniReddit.Models
{
    public class GroupChat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("createdbyid")]
        public string CreatedById { get; set; } = string.Empty;
        [JsonPropertyName("userids")]
        public List<string> UserIds { get; set; } = new List<string>();
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("userid")]
        public string UserId { get; set; } = string.Empty;
        [JsonPropertyName("chatid")]
        public string ChatId { get; set; } = string.Empty;
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        [JsonPropertyName("sentat")]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
