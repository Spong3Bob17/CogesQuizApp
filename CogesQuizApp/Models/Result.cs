using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CogesQuizApp.Models
{
    public class Result
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Score")]
        public string Score { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}