using CogesQuizApp.Models;
using System;
using System.Collections.Generic;

namespace CogesQuizApp.Tests.Helpers
{
    /// <summary>
    /// Classe helper con metodi di utilità per i test.
    /// Fornisce factory methods per creare oggetti di test comuni.
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Crea un Test di esempio con il numero specificato di domande
        /// </summary>
        /// <param name="title">Titolo del test</param>
        /// <param name="questionCount">Numero di domande</param>
        /// <param name="answersPerQuestion">Numero di risposte per domanda</param>
        /// <returns>Test completo</returns>
        public static Test CreateSampleTest(
            string title = "Test di Esempio", 
            int questionCount = 3, 
            int answersPerQuestion = 4)
        {
            var questions = new List<Question>();
            
            for (int i = 0; i < questionCount; i++)
            {
                var answers = new List<Answer>();
                for (int j = 0; j < answersPerQuestion; j++)
                {
                    answers.Add(new Answer { Text = $"Risposta {j + 1}" });
                }

                questions.Add(new Question
                {
                    Text = $"Domanda {i + 1}?",
                    Answers = answers,
                    CorrectAnswerIndex = 0 // Prima risposta corretta di default
                });
            }

            return new Test
            {
                Title = title,
                Questions = questions
            };
        }

        /// <summary>
        /// Crea un Result di esempio
        /// </summary>
        /// <param name="username">Nome utente</param>
        /// <param name="correctAnswers">Risposte corrette</param>
        /// <param name="totalQuestions">Totale domande</param>
        /// <param name="testId">ID del test (opzionale)</param>
        /// <returns>Result completo</returns>
        public static Result CreateSampleResult(
            string username = "TestUser",
            int correctAnswers = 5,
            int totalQuestions = 10,
            string testId = null)
        {
            return new Result
            {
                Username = username,
                TestId = testId ?? Guid.NewGuid().ToString(),
                TestTitle = "Test di Esempio",
                Score = $"{correctAnswers}/{totalQuestions}",
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions,
                Date = DateTime.UtcNow,
                SessionId = GenerateSessionId()
            };
        }

        /// <summary>
        /// Crea un UserAnswer di esempio
        /// </summary>
        /// <param name="username">Nome utente</param>
        /// <param name="questionIndex">Indice della domanda</param>
        /// <param name="isCorrect">Se la risposta è corretta</param>
        /// <param name="sessionId">ID sessione (opzionale)</param>
        /// <returns>UserAnswer completo</returns>
        public static UserAnswer CreateSampleUserAnswer(
            string username = "TestUser",
            int questionIndex = 0,
            bool isCorrect = true,
            string sessionId = null)
        {
            var correctIndex = 0;
            var selectedIndex = isCorrect ? correctIndex : 1;

            return new UserAnswer
            {
                Username = username,
                TestId = Guid.NewGuid().ToString(),
                TestTitle = "Test di Esempio",
                QuestionIndex = questionIndex,
                QuestionText = $"Domanda {questionIndex + 1}?",
                SelectedAnswerIndex = selectedIndex,
                SelectedAnswerText = $"Risposta {selectedIndex + 1}",
                CorrectAnswerIndex = correctIndex,
                IsCorrect = isCorrect,
                AnsweredAt = DateTime.UtcNow,
                SessionId = sessionId ?? GenerateSessionId()
            };
        }

        /// <summary>
        /// Crea una lista di UserAnswers per una sessione completa
        /// </summary>
        /// <param name="username">Nome utente</param>
        /// <param name="questionCount">Numero di domande</param>
        /// <param name="correctAnswers">Numero di risposte corrette</param>
        /// <param name="sessionId">ID sessione (opzionale)</param>
        /// <returns>Lista di UserAnswer</returns>
        public static List<UserAnswer> CreateSessionAnswers(
            string username = "TestUser",
            int questionCount = 5,
            int correctAnswers = 3,
            string sessionId = null)
        {
            sessionId ??= GenerateSessionId();
            var answers = new List<UserAnswer>();

            for (int i = 0; i < questionCount; i++)
            {
                // Le prime 'correctAnswers' domande saranno corrette
                bool isCorrect = i < correctAnswers;
                answers.Add(CreateSampleUserAnswer(username, i, isCorrect, sessionId));
            }

            return answers;
        }

        /// <summary>
        /// Genera un Session ID univoco
        /// </summary>
        /// <returns>Session ID nel formato session_{timestamp}_{random}</returns>
        public static string GenerateSessionId()
        {
            return $"session_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        /// <summary>
        /// Calcola la percentuale da una stringa score
        /// </summary>
        /// <param name="score">Score nel formato "5/10"</param>
        /// <returns>Percentuale (0-100)</returns>
        public static double CalculatePercentage(string score)
        {
            if (string.IsNullOrEmpty(score) || !score.Contains("/"))
                return 0;

            var parts = score.Split('/');
            if (parts.Length != 2)
                return 0;

            if (!int.TryParse(parts[0], out int correct) || 
                !int.TryParse(parts[1], out int total) || 
                total == 0)
                return 0;

            return (correct / (double)total) * 100;
        }

        /// <summary>
        /// Valida il formato di uno score
        /// </summary>
        /// <param name="score">Score da validare</param>
        /// <returns>True se il formato è valido</returns>
        public static bool IsValidScoreFormat(string score)
        {
            if (string.IsNullOrEmpty(score))
                return false;

            var parts = score.Split('/');
            if (parts.Length != 2)
                return false;

            return int.TryParse(parts[0], out int correct) && 
                   int.TryParse(parts[1], out int total) &&
                   correct >= 0 && total > 0 && correct <= total;
        }

        /// <summary>
        /// Crea una collezione di test per testare la paginazione
        /// </summary>
        /// <param name="count">Numero di test da creare</param>
        /// <returns>Lista di test</returns>
        public static List<Test> CreateTestCollection(int count)
        {
            var tests = new List<Test>();
            for (int i = 0; i < count; i++)
            {
                tests.Add(CreateSampleTest($"Test {i + 1}", 5, 4));
            }
            return tests;
        }

        /// <summary>
        /// Crea una collezione di risultati con date diverse per testare l'ordinamento
        /// </summary>
        /// <param name="count">Numero di risultati</param>
        /// <returns>Lista di risultati</returns>
        public static List<Result> CreateResultCollectionWithDates(int count)
        {
            var results = new List<Result>();
            var baseDate = DateTime.UtcNow;

            for (int i = 0; i < count; i++)
            {
                var result = CreateSampleResult($"User{i}", 5, 10);
                result.Date = baseDate.AddHours(-i); // Ogni risultato è un'ora prima del precedente
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Verifica se due date sono "vicine" (per gestire piccole differenze temporali nei test)
        /// </summary>
        /// <param name="date1">Prima data</param>
        /// <param name="date2">Seconda data</param>
        /// <param name="toleranceSeconds">Tolleranza in secondi</param>
        /// <returns>True se le date sono entro la tolleranza</returns>
        public static bool AreDatesClose(DateTime date1, DateTime date2, int toleranceSeconds = 5)
        {
            var diff = Math.Abs((date1 - date2).TotalSeconds);
            return diff <= toleranceSeconds;
        }
    }
}