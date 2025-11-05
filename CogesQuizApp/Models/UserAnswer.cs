using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CogesQuizApp.Models
{
    /// <summary>
    /// Rappresenta una singola risposta data da un utente durante un test.
    /// Ogni volta che l'utente risponde a una domanda, viene creato un record UserAnswer.
    /// </summary>
    public class UserAnswer
    {
        /// <summary>
        /// Identificatore univoco della risposta nel database
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Nome dell'utente che ha risposto
        /// </summary>
        [BsonElement("Username")]
        public string Username { get; set; }

        /// <summary>
        /// ID del test a cui appartiene questa risposta
        /// </summary>
        [BsonElement("TestId")]
        public string TestId { get; set; }

        /// <summary>
        /// Titolo del test (denormalizzato per query più veloci)
        /// </summary>
        [BsonElement("TestTitle")]
        public string TestTitle { get; set; }

        /// <summary>
        /// Indice della domanda nel test (0-based)
        /// </summary>
        [BsonElement("QuestionIndex")]
        public int QuestionIndex { get; set; }

        /// <summary>
        /// Testo della domanda
        /// </summary>
        [BsonElement("QuestionText")]
        public string QuestionText { get; set; }

        /// <summary>
        /// Indice della risposta selezionata dall'utente
        /// </summary>
        [BsonElement("SelectedAnswerIndex")]
        public int SelectedAnswerIndex { get; set; }

        /// <summary>
        /// Testo della risposta selezionata
        /// </summary>
        [BsonElement("SelectedAnswerText")]
        public string SelectedAnswerText { get; set; }

        /// <summary>
        /// Indice della risposta corretta
        /// </summary>
        [BsonElement("CorrectAnswerIndex")]
        public int CorrectAnswerIndex { get; set; }

        /// <summary>
        /// Indica se la risposta è corretta
        /// </summary>
        [BsonElement("IsCorrect")]
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Data e ora in cui è stata data la risposta
        /// </summary>
        [BsonElement("AnsweredAt")]
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID della sessione di test (per raggruppare tutte le risposte di un tentativo)
        /// </summary>
        [BsonElement("SessionId")]
        public string SessionId { get; set; }
    }
}