using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CogesQuizApp.Models
{
    /// <summary>
    /// Rappresenta il risultato finale di un test completato da un utente.
    /// Viene salvato alla fine del test con il punteggio totale.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Identificatore univoco del risultato
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Nome dell'utente che ha completato il test
        /// </summary>
        [BsonElement("Username")]
        public string Username { get; set; }

        /// <summary>
        /// ID del test completato
        /// </summary>
        [BsonElement("TestId")]
        public string TestId { get; set; }

        /// <summary>
        /// Titolo del test completato
        /// </summary>
        [BsonElement("TestTitle")]
        public string TestTitle { get; set; }

        /// <summary>
        /// Punteggio nel formato "risposte_corrette/totale_domande" (es. "5/10")
        /// </summary>
        [BsonElement("Score")]
        public string Score { get; set; }

        /// <summary>
        /// Numero di risposte corrette
        /// </summary>
        [BsonElement("CorrectAnswers")]
        public int CorrectAnswers { get; set; }

        /// <summary>
        /// Numero totale di domande nel test
        /// </summary>
        [BsonElement("TotalQuestions")]
        public int TotalQuestions { get; set; }

        /// <summary>
        /// Data e ora di completamento del test
        /// </summary>
        [BsonElement("Date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ID della sessione (per collegare il risultato alle singole risposte)
        /// </summary>
        [BsonElement("SessionId")]
        public string SessionId { get; set; }
    }
}