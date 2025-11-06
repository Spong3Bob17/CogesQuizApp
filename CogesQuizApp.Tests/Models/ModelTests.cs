using NUnit.Framework;
using FluentAssertions;
using CogesQuizApp.Models;
using System;
using System.Collections.Generic;

namespace CogesQuizApp.Tests.Models
{
    /// <summary>
    /// Test suite per verificare i modelli di dominio dell'applicazione.
    /// Testa la creazione, validazione e comportamento dei modelli.
    /// </summary>
    [TestFixture]
    public class ModelTests
    {
        #region Test Test Model

        /// <summary>
        /// Verifica che un Test può essere creato con tutte le proprietà corrette
        /// </summary>
        [Test]
        public void Test_CanBeCreated_WithValidProperties()
        {
            // Arrange & Act
            var test = new Test
            {
                Id = "test123",
                Title = "Test di Matematica",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Quanto fa 2+2?",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "3" },
                            new Answer { Text = "4" },
                            new Answer { Text = "5" }
                        },
                        CorrectAnswerIndex = 1
                    }
                }
            };

            // Assert
            test.Should().NotBeNull();
            test.Id.Should().Be("test123");
            test.Title.Should().Be("Test di Matematica");
            test.Questions.Should().HaveCount(1);
            test.Questions[0].Answers.Should().HaveCount(3);
        }

        /// <summary>
        /// Verifica che una Question può avere un numero variabile di risposte
        /// </summary>
        [Test]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(11)]
        public void Question_CanHaveVariableNumberOfAnswers(int answerCount)
        {
            // Arrange
            var answers = new List<Answer>();
            for (int i = 0; i < answerCount; i++)
            {
                answers.Add(new Answer { Text = $"Answer {i}" });
            }

            // Act
            var question = new Question
            {
                Text = "Test question",
                Answers = answers,
                CorrectAnswerIndex = 0
            };

            // Assert
            question.Answers.Should().HaveCount(answerCount);
        }

        /// <summary>
        /// Verifica che l'indice della risposta corretta è valido
        /// </summary>
        [Test]
        public void Question_CorrectAnswerIndex_ShouldBeWithinRange()
        {
            // Arrange
            var question = new Question
            {
                Text = "Test",
                Answers = new List<Answer>
                {
                    new Answer { Text = "A" },
                    new Answer { Text = "B" },
                    new Answer { Text = "C" }
                },
                CorrectAnswerIndex = 1
            };

            // Assert
            question.CorrectAnswerIndex.Should().BeGreaterOrEqualTo(0);
            question.CorrectAnswerIndex.Should().BeLessThan(question.Answers.Count);
        }

        #endregion

        #region Test Result Model

        /// <summary>
        /// Verifica che un Result può essere creato con tutte le proprietà
        /// </summary>
        [Test]
        public void Result_CanBeCreated_WithAllProperties()
        {
            // Arrange & Act
            var result = new Result
            {
                Id = "result123",
                Username = "Mario",
                TestId = "test123",
                TestTitle = "Test di Storia",
                Score = "8/10",
                CorrectAnswers = 8,
                TotalQuestions = 10,
                Date = DateTime.UtcNow,
                SessionId = "session_abc"
            };

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("Mario");
            result.Score.Should().Be("8/10");
            result.CorrectAnswers.Should().Be(8);
            result.TotalQuestions.Should().Be(10);
            result.SessionId.Should().Be("session_abc");
        }

        /// <summary>
        /// Verifica che il formato dello Score è corretto
        /// </summary>
        [Test]
        [TestCase("5/10")]
        [TestCase("10/10")]
        [TestCase("0/5")]
        public void Result_Score_ShouldHaveCorrectFormat(string score)
        {
            // Arrange & Act
            var result = new Result { Score = score };

            // Assert
            result.Score.Should().Contain("/");
            result.Score.Should().MatchRegex(@"^\d+/\d+$");
        }

        /// <summary>
        /// Verifica che la data viene impostata di default
        /// </summary>
        [Test]
        public void Result_Date_ShouldBeSetByDefault()
        {
            // Arrange & Act
            var result = new Result();

            // Assert
            result.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        #endregion

        #region Test UserAnswer Model

        /// <summary>
        /// Verifica che UserAnswer registra correttamente una risposta
        /// </summary>
        [Test]
        public void UserAnswer_CanBeCreated_WithCompleteInformation()
        {
            // Arrange & Act
            var userAnswer = new UserAnswer
            {
                Id = "answer123",
                Username = "Giulia",
                TestId = "test456",
                TestTitle = "Test di Geografia",
                QuestionIndex = 0,
                QuestionText = "Qual è la capitale d'Italia?",
                SelectedAnswerIndex = 2,
                SelectedAnswerText = "Roma",
                CorrectAnswerIndex = 2,
                IsCorrect = true,
                AnsweredAt = DateTime.UtcNow,
                SessionId = "session_xyz"
            };

            // Assert
            userAnswer.Should().NotBeNull();
            userAnswer.Username.Should().Be("Giulia");
            userAnswer.QuestionIndex.Should().Be(0);
            userAnswer.IsCorrect.Should().BeTrue();
            userAnswer.SelectedAnswerIndex.Should().Be(userAnswer.CorrectAnswerIndex);
        }

        /// <summary>
        /// Verifica che IsCorrect è false quando la risposta è sbagliata
        /// </summary>
        [Test]
        public void UserAnswer_IsCorrect_ShouldBeFalse_WhenAnswerIsWrong()
        {
            // Arrange & Act
            var userAnswer = new UserAnswer
            {
                SelectedAnswerIndex = 1,
                CorrectAnswerIndex = 2,
                IsCorrect = false
            };

            // Assert
            userAnswer.IsCorrect.Should().BeFalse();
            userAnswer.SelectedAnswerIndex.Should().NotBe(userAnswer.CorrectAnswerIndex);
        }

        /// <summary>
        /// Verifica che AnsweredAt viene impostato di default
        /// </summary>
        [Test]
        public void UserAnswer_AnsweredAt_ShouldBeSetByDefault()
        {
            // Arrange & Act
            var userAnswer = new UserAnswer();

            // Assert
            userAnswer.AnsweredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Verifica che il SessionId collega correttamente le risposte
        /// </summary>
        [Test]
        public void UserAnswer_SessionId_LinksMultipleAnswers()
        {
            // Arrange
            var sessionId = "session_test_123";

            // Act
            var answer1 = new UserAnswer { SessionId = sessionId, QuestionIndex = 0 };
            var answer2 = new UserAnswer { SessionId = sessionId, QuestionIndex = 1 };
            var answer3 = new UserAnswer { SessionId = sessionId, QuestionIndex = 2 };

            // Assert
            answer1.SessionId.Should().Be(sessionId);
            answer2.SessionId.Should().Be(sessionId);
            answer3.SessionId.Should().Be(sessionId);
        }

        #endregion

        #region Integration Tests Between Models

        /// <summary>
        /// Verifica l'integrazione tra Test e Result
        /// </summary>
        [Test]
        public void Integration_Test_And_Result_ShouldBeLinked()
        {
            // Arrange
            var test = new Test
            {
                Id = "test789",
                Title = "Test Integrazione",
                Questions = new List<Question>
                {
                    new Question { Text = "Q1", Answers = new List<Answer>() },
                    new Question { Text = "Q2", Answers = new List<Answer>() },
                    new Question { Text = "Q3", Answers = new List<Answer>() }
                }
            };

            // Act
            var result = new Result
            {
                TestId = test.Id,
                TestTitle = test.Title,
                TotalQuestions = test.Questions.Count,
                CorrectAnswers = 2,
                Score = "2/3"
            };

            // Assert
            result.TestId.Should().Be(test.Id);
            result.TestTitle.Should().Be(test.Title);
            result.TotalQuestions.Should().Be(test.Questions.Count);
        }

        /// <summary>
        /// Verifica che UserAnswer e Result appartengono alla stessa sessione
        /// </summary>
        [Test]
        public void Integration_UserAnswer_And_Result_ShareSameSession()
        {
            // Arrange
            var sessionId = "session_integration_test";

            // Act
            var userAnswer = new UserAnswer
            {
                SessionId = sessionId,
                QuestionIndex = 0,
                IsCorrect = true
            };

            var result = new Result
            {
                SessionId = sessionId,
                CorrectAnswers = 1,
                TotalQuestions = 1
            };

            // Assert
            userAnswer.SessionId.Should().Be(result.SessionId);
        }

        #endregion
    }
}