using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace CogesQuizApp.Models
{
    /// <summary>
    /// Rappresenta un test/quiz completo con tutte le sue domande.
    /// Ogni test è composto da un titolo e una lista di domande con relative risposte.
    /// </summary>
    public class Test
    {
        /// <summary>
        /// Identificatore univoco del test nel database MongoDB.
        /// Viene generato automaticamente da MongoDB come ObjectId.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Titolo del test visualizzato all'utente.
        /// Es: "Test di Matematica", "Quiz di Geografia"
        /// </summary>
        [BsonElement("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Lista delle domande che compongono il test.
        /// Ogni test può avere un numero illimitato di domande.
        /// Le domande vengono presentate nell'ordine della lista.
        /// </summary>
        [BsonElement("Questions")]
        public List<Question> Questions { get; set; }
    }

    /// <summary>
    /// Rappresenta una singola domanda all'interno di un test.
    /// Ogni domanda ha un testo, una lista di possibili risposte,
    /// e l'indice della risposta corretta.
    /// </summary>
    public class Question
    {
        /// <summary>
        /// Testo della domanda mostrato all'utente.
        /// Es: "Quanto fa 2+2?", "Qual è la capitale d'Italia?"
        /// </summary>
        [BsonElement("Text")]
        public string Text { get; set; }

        /// <summary>
        /// Lista delle possibili risposte per questa domanda.
        /// Il numero di risposte è variabile (minimo 2, nessun massimo).
        /// Questo soddisfa il requisito "2-n answer options".
        /// </summary>
        [BsonElement("Answers")]
        public List<Answer> Answers { get; set; }

        /// <summary>
        /// Indice della risposta corretta nell'array Answers (0-based).
        /// Es: Se CorrectAnswerIndex = 1, la seconda risposta è quella corretta.
        /// Utilizzato per verificare se l'utente ha risposto correttamente.
        /// </summary>
        [BsonElement("CorrectAnswerIndex")]
        public int CorrectAnswerIndex { get; set; }
    }

    /// <summary>
    /// Rappresenta una singola opzione di risposta per una domanda.
    /// Contiene solo il testo della risposta.
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// Testo della risposta mostrato all'utente come opzione selezionabile.
        /// Es: "4", "Roma", "Vero"
        /// </summary>
        [BsonElement("Text")]
        public string Text { get; set; }
    }
}