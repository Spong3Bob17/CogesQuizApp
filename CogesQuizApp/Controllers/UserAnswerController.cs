using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using CogesQuizApp.Models;
using CogesQuizApp.Services;

namespace CogesQuizApp.Controllers
{
    /// <summary>
    /// Controller per gestire le risposte singole degli utenti.
    /// Ogni volta che un utente risponde a una domanda, viene chiamato questo controller.
    /// </summary>
    public class UserAnswerController
    {
        private readonly IDatabaseService _dbService;

        public UserAnswerController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Gestisce le richieste HTTP per le risposte degli utenti
        /// </summary>
        /// <param name="context">Contesto della richiesta HTTP</param>
        public void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string path = request.Url?.AbsolutePath ?? "";

            try
            {
                // POST /user-answers - Salva una nuova risposta
                if (request.HttpMethod == "POST" && path == "/user-answers")
                {
                    HandleSaveAnswer(context);
                }
                // GET /user-answers/session/{sessionId} - Ottiene risposte per sessione
                else if (request.HttpMethod == "GET" && path.StartsWith("/user-answers/session/"))
                {
                    string sessionId = path.Replace("/user-answers/session/", "");
                    HandleGetBySession(context, sessionId);
                }
                // GET /user-answers?username=X&testId=Y - Ottiene risposte per utente e test
                else if (request.HttpMethod == "GET" && path == "/user-answers")
                {
                    string username = request.QueryString["username"];
                    string testId = request.QueryString["testId"];
                    
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(testId))
                    {
                        HandleGetByUserAndTest(context, username, testId);
                    }
                    else
                    {
                        SendResponse(response, 400, new { message = "Username and testId are required" });
                    }
                }
                else
                {
                    SendResponse(response, 404, new { message = "Endpoint not found" });
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Errore in UserAnswerController: {ex.Message}");
                Console.ResetColor();
                SendResponse(response, 500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Salva una nuova risposta nel database
        /// </summary>
        private void HandleSaveAnswer(HttpListenerContext context)
        {
            using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            string body = reader.ReadToEnd();

            // Deserializza la risposta
            var userAnswer = JsonSerializer.Deserialize<UserAnswer>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Validazione dei dati
            if (userAnswer == null)
            {
                SendResponse(context.Response, 400, new { message = "Invalid user answer data" });
                return;
            }

            if (string.IsNullOrWhiteSpace(userAnswer.Username))
            {
                SendResponse(context.Response, 400, new { message = "Username is required" });
                return;
            }

            if (string.IsNullOrWhiteSpace(userAnswer.TestId))
            {
                SendResponse(context.Response, 400, new { message = "TestId is required" });
                return;
            }

            if (string.IsNullOrWhiteSpace(userAnswer.SessionId))
            {
                SendResponse(context.Response, 400, new { message = "SessionId is required" });
                return;
            }

            // Salva nel database
            _dbService.SaveUserAnswer(userAnswer);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"📝 Risposta salvata: {userAnswer.Username} -> Q{userAnswer.QuestionIndex} -> " +
                            $"A{userAnswer.SelectedAnswerIndex} {(userAnswer.IsCorrect ? "✓" : "✗")}");
            Console.ResetColor();

            SendResponse(context.Response, 201, new 
            { 
                message = "Answer saved successfully",
                isCorrect = userAnswer.IsCorrect
            });
        }

        /// <summary>
        /// Recupera tutte le risposte di una sessione specifica
        /// </summary>
        private void HandleGetBySession(HttpListenerContext context, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                SendResponse(context.Response, 400, new { message = "SessionId is required" });
                return;
            }

            var answers = _dbService.GetAnswersBySession(sessionId);
            SendResponse(context.Response, 200, answers);
        }

        /// <summary>
        /// Recupera le risposte di un utente per un test specifico
        /// </summary>
        private void HandleGetByUserAndTest(HttpListenerContext context, string username, string testId)
        {
            var answers = _dbService.GetAnswersByUserAndTest(username, testId);
            SendResponse(context.Response, 200, answers);
        }

        /// <summary>
        /// Invia una risposta HTTP in formato JSON
        /// </summary>
        private void SendResponse(HttpListenerResponse response, int statusCode, object data)
        {
            // Opzioni di serializzazione con supporto UTF-8 per caratteri accentati
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            
            string json = JsonSerializer.Serialize(data, options);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }
    }
}