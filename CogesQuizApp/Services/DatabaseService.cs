using CogesQuizApp.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace CogesQuizApp.Services
{
    /// <summary>
    /// Servizio per la gestione delle operazioni sul database MongoDB.
    /// Gestisce le connessioni, le query e gli indici per ottimizzare le performance.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Inizializza il servizio database e crea gli indici necessari
        /// </summary>
        /// <param name="connectionString">Stringa di connessione MongoDB</param>
        /// <param name="databaseName">Nome del database</param>
        public DatabaseService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            
            // Crea gli indici per ottimizzare le query
            CreateIndexes();
        }

        /// <summary>
        /// Collezione dei test disponibili
        /// </summary>
        public IMongoCollection<Test> Tests => _database.GetCollection<Test>("tests");

        /// <summary>
        /// Collezione dei risultati finali
        /// </summary>
        public IMongoCollection<Result> Results => _database.GetCollection<Result>("results");

        /// <summary>
        /// Collezione delle singole risposte degli utenti
        /// </summary>
        public IMongoCollection<UserAnswer> UserAnswers => _database.GetCollection<UserAnswer>("user_answers");

        /// <summary>
        /// Crea gli indici necessari per ottimizzare le query.
        /// Gli indici sono fondamentali per mantenere buone performance quando il database cresce.
        /// </summary>
        private void CreateIndexes()
        {
            try
            {
                // Indice su Results per query ordinate per data (leaderboard)
                var resultIndexKeys = Builders<Result>.IndexKeys
                    .Descending(r => r.Date)
                    .Ascending(r => r.Username);
                Results.Indexes.CreateOne(new CreateIndexModel<Result>(resultIndexKeys));

                // Indice su UserAnswers per query per utente e test
                var userAnswerIndexKeys = Builders<UserAnswer>.IndexKeys
                    .Ascending(ua => ua.Username)
                    .Ascending(ua => ua.TestId)
                    .Ascending(ua => ua.AnsweredAt);
                UserAnswers.Indexes.CreateOne(new CreateIndexModel<UserAnswer>(userAnswerIndexKeys));

                // Indice su UserAnswers per SessionId (per recuperare tutte le risposte di una sessione)
                var sessionIndexKeys = Builders<UserAnswer>.IndexKeys.Ascending(ua => ua.SessionId);
                UserAnswers.Indexes.CreateOne(new CreateIndexModel<UserAnswer>(sessionIndexKeys));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ Indici database creati con successo");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  Avviso: impossibile creare gli indici - {ex.Message}");
                Console.ResetColor();
            }
        }

        // ========== METODI PER I TEST ==========

        /// <summary>
        /// Recupera tutti i test disponibili nel database
        /// </summary>
        /// <returns>Lista di tutti i test</returns>
        public List<Test> GetAllTests()
        {
            return Tests.Find(_ => true).ToList();
        }

        /// <summary>
        /// Recupera un test specifico tramite il suo ID
        /// </summary>
        /// <param name="id">ID del test da recuperare</param>
        /// <returns>Il test richiesto o null se non trovato</returns>
        public Test GetTestById(string id)
        {
            return Tests.Find(t => t.Id == id).FirstOrDefault();
        }

        // ========== METODI PER I RISULTATI ==========

        /// <summary>
        /// Salva il risultato finale di un test completato
        /// </summary>
        /// <param name="result">Oggetto Result da salvare</param>
        public void SaveResult(Result result)
        {
            // Assicurati che la data sia impostata
            if (result.Date == DateTime.MinValue)
            {
                result.Date = DateTime.UtcNow;
            }
            
            Results.InsertOne(result);
        }

        /// <summary>
        /// Recupera tutti i risultati ordinati per data (più recenti prima)
        /// </summary>
        /// <returns>Lista di tutti i risultati ordinata per data</returns>
        public List<Result> GetAllResults()
        {
            return Results
                .Find(_ => true)
                .SortByDescending(r => r.Date)
                .ToList();
        }

        /// <summary>
        /// Recupera i risultati di un utente specifico
        /// </summary>
        /// <param name="username">Nome utente</param>
        /// <returns>Lista dei risultati dell'utente</returns>
        public List<Result> GetResultsByUsername(string username)
        {
            return Results
                .Find(r => r.Username == username)
                .SortByDescending(r => r.Date)
                .ToList();
        }

        // ========== METODI PER LE RISPOSTE SINGOLE ==========

        /// <summary>
        /// Salva una singola risposta data dall'utente durante un test.
        /// Questo metodo deve essere chiamato ogni volta che l'utente risponde a una domanda.
        /// </summary>
        /// <param name="userAnswer">Oggetto UserAnswer da salvare</param>
        public void SaveUserAnswer(UserAnswer userAnswer)
        {
            // Assicurati che la data sia impostata
            if (userAnswer.AnsweredAt == DateTime.MinValue)
            {
                userAnswer.AnsweredAt = DateTime.UtcNow;
            }

            UserAnswers.InsertOne(userAnswer);
        }

        /// <summary>
        /// Recupera tutte le risposte di una specifica sessione di test
        /// </summary>
        /// <param name="sessionId">ID della sessione</param>
        /// <returns>Lista delle risposte nella sessione</returns>
        public List<UserAnswer> GetAnswersBySession(string sessionId)
        {
            return UserAnswers
                .Find(ua => ua.SessionId == sessionId)
                .SortBy(ua => ua.QuestionIndex)
                .ToList();
        }

        /// <summary>
        /// Recupera tutte le risposte di un utente per un test specifico
        /// </summary>
        /// <param name="username">Nome utente</param>
        /// <param name="testId">ID del test</param>
        /// <returns>Lista delle risposte dell'utente per il test</returns>
        public List<UserAnswer> GetAnswersByUserAndTest(string username, string testId)
        {
            return UserAnswers
                .Find(ua => ua.Username == username && ua.TestId == testId)
                .SortByDescending(ua => ua.AnsweredAt)
                .ToList();
        }

        /// <summary>
        /// Recupera statistiche aggregate per un test specifico
        /// </summary>
        /// <param name="testId">ID del test</param>
        /// <returns>Numero totale di tentativi per questo test</returns>
        public long GetTestAttemptCount(string testId)
        {
            return Results.CountDocuments(r => r.TestId == testId);
        }
    }
}