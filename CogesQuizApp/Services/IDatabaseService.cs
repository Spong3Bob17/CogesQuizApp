using CogesQuizApp.Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace CogesQuizApp.Services
{
    /// <summary>
    /// Interfaccia per il servizio database.
    /// Permette il testing tramite mock.
    /// </summary>
    public interface IDatabaseService
    {
        // Collezioni
        IMongoCollection<Test> Tests { get; }
        IMongoCollection<Result> Results { get; }
        IMongoCollection<UserAnswer> UserAnswers { get; }

        // Metodi per Tests
        List<Test> GetAllTests();
        Test GetTestById(string id);

        // Metodi per Results
        void SaveResult(Result result);
        List<Result> GetAllResults();
        List<Result> GetResultsByUsername(string username);

        // Metodi per UserAnswers
        void SaveUserAnswer(UserAnswer userAnswer);
        List<UserAnswer> GetAnswersBySession(string sessionId);
        List<UserAnswer> GetAnswersByUserAndTest(string username, string testId);

        // Statistiche
        long GetTestAttemptCount(string testId);
    }
}