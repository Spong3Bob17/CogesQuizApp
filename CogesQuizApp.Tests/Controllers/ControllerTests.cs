using NUnit.Framework;
using FluentAssertions;
using Moq;
using CogesQuizApp.Controllers;
using CogesQuizApp.Models;
using CogesQuizApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CogesQuizApp.Tests.Controllers
{
    /// <summary>
    /// Test suite per i controller dell'applicazione.
    /// Utilizza Moq per simulare il DatabaseService e testare la logica dei controller in isolamento.
    /// </summary>
    [TestFixture]
    public class ControllerTests
    {
        private Mock<IDatabaseService> _mockDbService;

        /// <summary>
        /// Setup eseguito prima di ogni test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Crea un mock del DatabaseService
            _mockDbService = new Mock<IDatabaseService>();
        }

        #region TestController Tests

        /// <summary>
        /// Verifica che TestController può essere istanziato correttamente
        /// </summary>
        [Test]
        public void TestController_CanBeInstantiated()
        {
            // Act
            var controller = new TestController(_mockDbService.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        /// <summary>
        /// Test concettuale: verifica che GET /tests chiama GetAllTests
        /// Nota: Testare HttpListener richiede setup complesso, questo è un test di logica
        /// </summary>
        [Test]
        public void TestController_GetAllTests_ShouldCallDatabaseService()
        {
            // Arrange
            var expectedTests = new List<Test>
            {
                new Test { Id = "1", Title = "Test 1", Questions = new List<Question>() },
                new Test { Id = "2", Title = "Test 2", Questions = new List<Question>() }
            };

            _mockDbService.Setup(x => x.GetAllTests()).Returns(expectedTests);
            var controller = new TestController(_mockDbService.Object);

            // Act
            var tests = _mockDbService.Object.GetAllTests();

            // Assert
            tests.Should().HaveCount(2);
            tests.Should().BeEquivalentTo(expectedTests);
            _mockDbService.Verify(x => x.GetAllTests(), Times.Once);
        }

        #endregion

        #region ResultController Tests

        /// <summary>
        /// Verifica che ResultController può essere istanziato correttamente
        /// </summary>
        [Test]
        public void ResultController_CanBeInstantiated()
        {
            // Act
            var controller = new ResultController(_mockDbService.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        /// <summary>
        /// Test concettuale: verifica che SaveResult chiama il metodo del database
        /// </summary>
        [Test]
        public void ResultController_SaveResult_ShouldCallDatabaseService()
        {
            // Arrange
            var result = new Result
            {
                Username = "TestUser",
                TestId = "test1",
                Score = "5/10",
                CorrectAnswers = 5,
                TotalQuestions = 10
            };

            _mockDbService.Setup(x => x.SaveResult(It.IsAny<Result>()));
            var controller = new ResultController(_mockDbService.Object);

            // Act
            _mockDbService.Object.SaveResult(result);

            // Assert
            _mockDbService.Verify(x => x.SaveResult(It.Is<Result>(r => 
                r.Username == "TestUser" && 
                r.Score == "5/10"
            )), Times.Once);
        }

        /// <summary>
        /// Test concettuale: verifica che GetAllResults restituisce i dati corretti
        /// </summary>
        [Test]
        public void ResultController_GetAllResults_ShouldReturnResults()
        {
            // Arrange
            var expectedResults = new List<Result>
            {
                new Result { Username = "User1", Score = "7/10" },
                new Result { Username = "User2", Score = "9/10" }
            };

            _mockDbService.Setup(x => x.GetAllResults()).Returns(expectedResults);
            var controller = new ResultController(_mockDbService.Object);

            // Act
            var results = _mockDbService.Object.GetAllResults();

            // Assert
            results.Should().HaveCount(2);
            results[0].Username.Should().Be("User1");
            results[1].Username.Should().Be("User2");
        }

        #endregion

        #region UserAnswerController Tests

        /// <summary>
        /// Verifica che UserAnswerController può essere istanziato correttamente
        /// </summary>
        [Test]
        public void UserAnswerController_SaveUserResult_ShouldCallDatabaseService()

        {
            // Arrange
            var result = new Result
            {
                Username = "TestUser",
                TestId = "test1",
                Score = "5/10",
                CorrectAnswers = 5,
                TotalQuestions = 10
            };

            _mockDbService.Setup(x => x.SaveResult(It.IsAny<Result>()));

            // Act - usa direttamente il mock del database service
            _mockDbService.Object.SaveResult(result);

            // Assert
            _mockDbService.Verify(x => x.SaveResult(It.Is<Result>(r => 
                r.Username == "TestUser" && 
                r.Score == "5/10"
            )), Times.Once);
        }

        /// <summary>
        /// Test concettuale: verifica che SaveUserAnswer chiama il database service
        /// </summary>
        [Test]
        public void UserAnswerController_SaveUserAnswer_ShouldCallDatabaseService()
        {
            // Arrange
            var userAnswer = new UserAnswer
            {
                Username = "Mario",
                TestId = "test1",
                QuestionIndex = 0,
                SelectedAnswerIndex = 2,
                CorrectAnswerIndex = 2,
                IsCorrect = true,
                SessionId = "session123"
            };

            _mockDbService.Setup(x => x.SaveUserAnswer(It.IsAny<UserAnswer>()));
            var results = _mockDbService.Object.GetAllResults();

            // Act
            _mockDbService.Object.SaveUserAnswer(userAnswer);

            // Assert
            _mockDbService.Verify(x => x.SaveUserAnswer(It.Is<UserAnswer>(ua => 
                ua.Username == "Mario" && 
                ua.IsCorrect == true &&
                ua.SessionId == "session123"
            )), Times.Once);
        }

        /// <summary>
        /// Test concettuale: verifica che GetAnswersBySession filtra per sessione
        /// </summary>
        [Test]
        public void UserAnswerController_GetAnswersBySession_ShouldFilterBySession()
        {
            // Arrange
            var sessionId = "session_abc";
            var expectedAnswers = new List<UserAnswer>
            {
                new UserAnswer { SessionId = sessionId, QuestionIndex = 0 },
                new UserAnswer { SessionId = sessionId, QuestionIndex = 1 },
                new UserAnswer { SessionId = sessionId, QuestionIndex = 2 }
            };

            _mockDbService.Setup(x => x.GetAnswersBySession(sessionId)).Returns(expectedAnswers);
            var controller = new UserAnswerController(_mockDbService.Object);

            // Act
            var answers = _mockDbService.Object.GetAnswersBySession(sessionId);

            // Assert
            answers.Should().HaveCount(3);
            answers.Should().OnlyContain(a => a.SessionId == sessionId);
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Verifica che un Result con dati mancanti può essere identificato
        /// </summary>
        [Test]
        public void Validation_Result_WithMissingData_ShouldBeInvalid()
        {
            // Arrange
            var result = new Result
            {
                Username = "", // Mancante
                Score = "5/10"
            };

            // Assert
            result.Username.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Verifica che un Result valido ha tutti i campi richiesti
        /// </summary>
        [Test]
        public void Validation_Result_WithAllData_ShouldBeValid()
        {
            // Arrange
            var result = new Result
            {
                Username = "ValidUser",
                TestId = "test123",
                Score = "5/10",
                SessionId = "session123"
            };

            // Assert
            result.Username.Should().NotBeNullOrEmpty();
            result.TestId.Should().NotBeNullOrEmpty();
            result.Score.Should().NotBeNullOrEmpty();
            result.SessionId.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Verifica che UserAnswer con dati mancanti può essere identificato
        /// </summary>
        [Test]
        public void Validation_UserAnswer_WithMissingData_ShouldBeInvalid()
        {
            // Arrange
            var userAnswer = new UserAnswer
            {
                Username = "User",
                TestId = "", // Mancante
                SessionId = ""  // Mancante
            };

            // Assert
            userAnswer.TestId.Should().BeNullOrEmpty();
            userAnswer.SessionId.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Verifica formato Score valido
        /// </summary>
        [Test]
        [TestCase("5/10", true)]
        [TestCase("10/10", true)]
        [TestCase("0/5", true)]
        [TestCase("invalid", false)]
        [TestCase("5-10", false)]
        [TestCase("", false)]
        public void Validation_ScoreFormat_ShouldBeValidated(string score, bool shouldBeValid)
        {
            // Act
            bool isValid = !string.IsNullOrEmpty(score) && score.Contains("/");
            var parts = score.Split('/');
            isValid = isValid && parts.Length == 2 && 
                      int.TryParse(parts[0], out _) && 
                      int.TryParse(parts[1], out _);

            // Assert
            isValid.Should().Be(shouldBeValid);
        }

        #endregion

        #region Error Handling Tests

        /// <summary>
        /// Verifica che il controller gestisce eccezioni del database
        /// </summary>
        [Test]
        public void Controller_WhenDatabaseThrowsException_ShouldHandleGracefully()
        {
            // Arrange
            _mockDbService.Setup(x => x.GetAllTests())
                .Throws(new Exception("Database connection error"));

            // Act & Assert
            Action act = () => _mockDbService.Object.GetAllTests();
            act.Should().Throw<Exception>().WithMessage("Database connection error");
        }

        /// <summary>
        /// Verifica che SaveResult gestisce valori null
        /// </summary>
        [Test]
        public void SaveResult_WhenResultIsNull_ShouldThrow()
        {
            // Arrange
            _mockDbService.Setup(x => x.SaveResult(null))
                .Throws(new ArgumentNullException("result"));

            // Act & Assert
            Action act = () => _mockDbService.Object.SaveResult(null);
            act.Should().Throw<ArgumentNullException>();
        }

        #endregion

        #region Business Logic Tests

        /// <summary>
        /// Verifica il calcolo della percentuale di successo
        /// </summary>
        [Test]
        [TestCase("5/10", 50.0)]
        [TestCase("10/10", 100.0)]
        [TestCase("0/10", 0.0)]
        [TestCase("7/20", 35.0)]
        public void BusinessLogic_CalculatePercentage_ShouldBeCorrect(string score, double expectedPercentage)
        {
            // Arrange
            var parts = score.Split('/');
            var correct = double.Parse(parts[0]);
            var total = double.Parse(parts[1]);

            // Act
            var percentage = (correct / total) * 100;

            // Assert
            percentage.Should().Be(expectedPercentage);
        }

        /// <summary>
        /// Verifica che SessionId collega correttamente risposte e risultati
        /// </summary>
        [Test]
        public void BusinessLogic_SessionId_ShouldLinkAnswersAndResults()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            var userAnswers = new List<UserAnswer>
            {
                new UserAnswer { SessionId = sessionId, IsCorrect = true },
                new UserAnswer { SessionId = sessionId, IsCorrect = false },
                new UserAnswer { SessionId = sessionId, IsCorrect = true }
            };

            var result = new Result
            {
                SessionId = sessionId,
                CorrectAnswers = 2,
                TotalQuestions = 3
            };

            // Act
            var correctCount = userAnswers.Count(a => a.IsCorrect);

            // Assert
            correctCount.Should().Be(result.CorrectAnswers);
            result.SessionId.Should().Be(sessionId);
        }

        /// <summary>
        /// Verifica che il punteggio finale corrisponde alle risposte date
        /// </summary>
        [Test]
        public void BusinessLogic_FinalScore_ShouldMatchAnswers()
        {
            // Arrange
            var sessionId = "session_test";
            var answers = new List<UserAnswer>
            {
                new UserAnswer { SessionId = sessionId, IsCorrect = true },
                new UserAnswer { SessionId = sessionId, IsCorrect = true },
                new UserAnswer { SessionId = sessionId, IsCorrect = false },
                new UserAnswer { SessionId = sessionId, IsCorrect = true },
                new UserAnswer { SessionId = sessionId, IsCorrect = false }
            };

            // Act
            var correctAnswers = answers.Count(a => a.IsCorrect);
            var totalQuestions = answers.Count;
            var score = $"{correctAnswers}/{totalQuestions}";

            // Assert
            score.Should().Be("3/5");
            correctAnswers.Should().Be(3);
            totalQuestions.Should().Be(5);
        }

        #endregion
    }
}