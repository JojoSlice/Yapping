﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace miniReddit.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("userid")]
        public string UserId { get; set; }

        [JsonPropertyName("postid")]
        public string PostId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Comment() { }

        public Comment(string userid, string postid, string content)
        {
            UserId = userid;
            PostId = postid;
            Text = content;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
