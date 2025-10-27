using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace CogesQuizApp.Models
{
    public class Test
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Questions")]
        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("Answers")]
        public List<Answer> Answers { get; set; }

        [BsonElement("CorrectAnswerIndex")]
        public int CorrectAnswerIndex { get; set; }
    }

    public class Answer
    {
        [BsonElement("Text")]
        public string Text { get; set; }
    }
}