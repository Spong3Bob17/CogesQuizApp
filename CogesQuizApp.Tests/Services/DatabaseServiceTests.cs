using NUnit.Framework;
using FluentAssertions;
using CogesQuizApp.Models;
using CogesQuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace CogesQuizApp.Tests.Services
{
    /// <summary>
    /// Test suite per DatabaseService.
    /// Nota: Questi test richiedono MongoDB in esecuzione locale.
    /// Per test completamente isolati, considera l'uso di un database in-memory o mock.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class DatabaseServiceTests
    {
        private DatabaseService _dbService;
        private const string TestConnectionString = "mongodb://localhost:27017";
        private const string TestDatabaseName = "CogesQuizDB_Test";

        /// <summary>
        /// Setup eseguito prima di ogni test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _dbService = new DatabaseService(TestConnectionString, TestDatabaseName);
            
            // Pulisci il database di test prima di ogni test
            CleanupTestDatabase();
        }

        /// <summary>
        /// Cleanup eseguito dopo ogni test
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            CleanupTestDatabase();
        }

        /// <summary>
        /// Pulisce il database di test
        /// </summary>
        private void CleanupTestDatabase()
        {
            try
            {
                // Elimina tutti i documenti dalle collezioni di test
                _dbService.Tests.DeleteMany(_ => true);
                _dbService.Results.DeleteMany(_ => true);
                _dbService.UserAnswers.DeleteMany(_ => true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not cleanup test database: {ex.Message}");
            }
        }

        #region Test CRUD Operations - Tests

        /// <summary>
        /// Verifica che GetAllTests restituisce una lista vuota quando non ci sono test
        /// </summary>
        [Test]
        public void GetAllTests_WhenNoTests_ShouldReturnEmptyList()
        {
            // Act
            var tests = _dbService.GetAllTests();

            // Assert
            tests.Should().NotBeNull();
            tests.Should().BeEmpty();
        }

        /// <summary>
        /// Verifica che GetAllTests restituisce tutti i test inseriti
        /// </summary>
        [Test]
        public void GetAllTests_WhenTestsExist_ShouldReturnAllTests()
        {
            // Arrange
            var test1 = CreateSampleTest("Test 1");
            var test2 = CreateSampleTest("Test 2");
            _dbService.Tests.InsertOne(test1);
            _dbService.Tests.InsertOne(test2);

            // Act
            var tests = _dbService.GetAllTests();

            // Assert
            tests.Should().HaveCount(2);
            tests.Should().Contain(t => t.Title == "Test 1");
            tests.Should().Contain(t => t.Title == "Test 2");
        }

        /// <summary>
        /// Verifica che GetTestById restituisce il test corretto
        /// </summary>
        [Test]
        public void GetTestById_WhenTestExists_ShouldReturnTest()
        {
            // Arrange
            var test = CreateSampleTest("Test Specifico");
            _dbService.Tests.InsertOne(test);

            // Act
            var retrievedTest = _dbService.GetTestById(test.Id);

            // Assert
            retrievedTest.Should().NotBeNull();
            retrievedTest.Id.Should().Be(test.Id);
            retrievedTest.Title.Should().Be("Test Specifico");
        }

        /// <summary>
        /// Verifica che GetTestById restituisce null quando il test non esiste
        /// </summary>
        [Test]
        public void GetTestById_WhenTestDoesNotExist_ShouldReturnNull()
        {
            // Arrange - Crea un ObjectId valido ma che non esiste nel database
            var nonExistentId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            // Act
            var test = _dbService.GetTestById(nonExistentId);

            // Assert
            test.Should().BeNull();
        }

        #endregion

        #region Test CRUD Operations - Results

        /// <summary>
        /// Verifica che SaveResult salva correttamente un risultato
        /// </summary>
        [Test]
        public void SaveResult_ShouldSaveResultSuccessfully()
        {
            // Arrange
            var result = CreateSampleResult("TestUser", "5/10");

            // Act
            _dbService.SaveResult(result);
            var allResults = _dbService.GetAllResults();

            // Assert
            allResults.Should().HaveCount(1);
            allResults[0].Username.Should().Be("TestUser");
            allResults[0].Score.Should().Be("5/10");
        }

        /// <summary>
        /// Verifica che SaveResult imposta la data se non fornita
        /// </summary>
        [Test]
        public void SaveResult_WhenDateNotSet_ShouldSetDefaultDate()
        {
            // Arrange
            var result = new Result
            {
                Username = "User",
                Score = "1/1",
                Date = DateTime.MinValue // Data non impostata
            };

            // Act
            _dbService.SaveResult(result);
            var savedResult = _dbService.GetAllResults()[0];

            // Assert
            savedResult.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Verifica che GetAllResults restituisce risultati ordinati per data
        /// </summary>
        [Test]
        public void GetAllResults_ShouldReturnResultsOrderedByDate()
        {
            // Arrange
            var result1 = CreateSampleResult("User1", "5/10");
            result1.Date = DateTime.UtcNow.AddHours(-2);
            
            var result2 = CreateSampleResult("User2", "7/10");
            result2.Date = DateTime.UtcNow.AddHours(-1);
            
            var result3 = CreateSampleResult("User3", "9/10");
            result3.Date = DateTime.UtcNow;

            _dbService.SaveResult(result1);
            _dbService.SaveResult(result2);
            _dbService.SaveResult(result3);

            // Act
            var results = _dbService.GetAllResults();

            // Assert
            results.Should().HaveCount(3);
            results[0].Username.Should().Be("User3"); // Più recente
            results[1].Username.Should().Be("User2");
            results[2].Username.Should().Be("User1"); // Meno recente
        }

        /// <summary>
        /// Verifica che GetResultsByUsername filtra correttamente per utente
        /// </summary>
        [Test]
        public void GetResultsByUsername_ShouldFilterByUsername()
        {
            // Arrange
            _dbService.SaveResult(CreateSampleResult("Mario", "5/10"));
            _dbService.SaveResult(CreateSampleResult("Luigi", "7/10"));
            _dbService.SaveResult(CreateSampleResult("Mario", "8/10"));

            // Act
            var marioResults = _dbService.GetResultsByUsername("Mario");

            // Assert
            marioResults.Should().HaveCount(2);
            marioResults.Should().OnlyContain(r => r.Username == "Mario");
        }

        #endregion

        #region Test CRUD Operations - UserAnswers

        /// <summary>
        /// Verifica che SaveUserAnswer salva correttamente una risposta
        /// </summary>
        [Test]
        public void SaveUserAnswer_ShouldSaveAnswerSuccessfully()
        {
            // Arrange
            var userAnswer = CreateSampleUserAnswer("User1", 0, true, "session1");

            // Act
            _dbService.SaveUserAnswer(userAnswer);
            var answers = _dbService.GetAnswersBySession("session1");

            // Assert
            answers.Should().HaveCount(1);
            answers[0].Username.Should().Be("User1");
            answers[0].IsCorrect.Should().BeTrue();
        }

        /// <summary>
        /// Verifica che SaveUserAnswer imposta la data se non fornita
        /// </summary>
        [Test]
        public void SaveUserAnswer_WhenDateNotSet_ShouldSetDefaultDate()
        {
            // Arrange
            var userAnswer = new UserAnswer
            {
                Username = "User",
                TestId = "test1",
                QuestionIndex = 0,
                IsCorrect = true,
                SessionId = "session1",
                AnsweredAt = DateTime.MinValue
            };

            // Act
            _dbService.SaveUserAnswer(userAnswer);
            var savedAnswer = _dbService.GetAnswersBySession("session1")[0];

            // Assert
            savedAnswer.AnsweredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Verifica che GetAnswersBySession recupera tutte le risposte di una sessione
        /// </summary>
        [Test]
        public void GetAnswersBySession_ShouldReturnAllAnswersForSession()
        {
            // Arrange
            var sessionId = "session_abc";
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User1", 0, true, sessionId));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User1", 1, false, sessionId));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User1", 2, true, sessionId));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User2", 0, true, "different_session"));

            // Act
            var answers = _dbService.GetAnswersBySession(sessionId);

            // Assert
            answers.Should().HaveCount(3);
            answers.Should().OnlyContain(a => a.SessionId == sessionId);
        }

        /// <summary>
        /// Verifica che GetAnswersBySession restituisce risposte ordinate per indice domanda
        /// </summary>
        [Test]
        public void GetAnswersBySession_ShouldReturnAnswersOrderedByQuestionIndex()
        {
            // Arrange
            var sessionId = "ordered_session";
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User", 2, true, sessionId));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User", 0, false, sessionId));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("User", 1, true, sessionId));

            // Act
            var answers = _dbService.GetAnswersBySession(sessionId);

            // Assert
            answers.Should().HaveCount(3);
            answers[0].QuestionIndex.Should().Be(0);
            answers[1].QuestionIndex.Should().Be(1);
            answers[2].QuestionIndex.Should().Be(2);
        }

        /// <summary>
        /// Verifica che GetAnswersByUserAndTest filtra correttamente
        /// </summary>
        [Test]
        public void GetAnswersByUserAndTest_ShouldFilterCorrectly()
        {
            // Arrange
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("Mario", 0, true, "s1", "test1"));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("Mario", 1, false, "s1", "test1"));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("Luigi", 0, true, "s2", "test1"));
            _dbService.SaveUserAnswer(CreateSampleUserAnswer("Mario", 0, true, "s3", "test2"));

            // Act
            var marioTest1Answers = _dbService.GetAnswersByUserAndTest("Mario", "test1");

            // Assert
            marioTest1Answers.Should().HaveCount(2);
            marioTest1Answers.Should().OnlyContain(a => a.Username == "Mario" && a.TestId == "test1");
        }

        #endregion

        #region Test Statistics Methods

        /// <summary>
        /// Verifica che GetTestAttemptCount conta correttamente i tentativi
        /// </summary>
        [Test]
        public void GetTestAttemptCount_ShouldCountCorrectly()
        {
            // Arrange
            var result1 = CreateSampleResult("User1", "5/10");
            result1.TestId = "test123";
            var result2 = CreateSampleResult("User2", "7/10");
            result2.TestId = "test123";
            var result3 = CreateSampleResult("User3", "9/10");
            result3.TestId = "test456";

            _dbService.SaveResult(result1);
            _dbService.SaveResult(result2);
            _dbService.SaveResult(result3);

            // Act
            var count = _dbService.GetTestAttemptCount("test123");

            // Assert
            count.Should().Be(2);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Crea un test di esempio per i test
        /// </summary>
        private Test CreateSampleTest(string title)
        {
            return new Test
            {
                Title = title,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Sample Question",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Answer 1" },
                            new Answer { Text = "Answer 2" }
                        },
                        CorrectAnswerIndex = 0
                    }
                }
            };
        }

        /// <summary>
        /// Crea un risultato di esempio per i test
        /// </summary>
        private Result CreateSampleResult(string username, string score)
        {
            return new Result
            {
                Username = username,
                TestId = "test_sample",
                TestTitle = "Sample Test",
                Score = score,
                CorrectAnswers = int.Parse(score.Split('/')[0]),
                TotalQuestions = int.Parse(score.Split('/')[1]),
                Date = DateTime.UtcNow,
                SessionId = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Crea una risposta utente di esempio per i test
        /// </summary>
        private UserAnswer CreateSampleUserAnswer(string username, int questionIndex, bool isCorrect, string sessionId, string testId = "test_sample")
        {
            return new UserAnswer
            {
                Username = username,
                TestId = testId,
                TestTitle = "Sample Test",
                QuestionIndex = questionIndex,
                QuestionText = $"Question {questionIndex}",
                SelectedAnswerIndex = isCorrect ? 0 : 1,
                SelectedAnswerText = isCorrect ? "Correct" : "Wrong",
                CorrectAnswerIndex = 0,
                IsCorrect = isCorrect,
                AnsweredAt = DateTime.UtcNow,
                SessionId = sessionId
            };
        }

        #endregion
    }
}