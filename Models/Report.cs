using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace miniReddit.Models
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("postid")]
        public string PostId { get; set; } = string.Empty;
        [JsonPropertyName("commentid")]
        public string CommentId { get; set; } = string.Empty;
        [JsonPropertyName("isread")]
        public bool IsRead { get; set; } = false;

    }
}
