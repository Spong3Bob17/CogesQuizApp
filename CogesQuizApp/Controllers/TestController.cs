using System;
using System.Net;
using System.Text;
using System.Text.Json;
using CogesQuizApp.Services;

namespace CogesQuizApp.Controllers
{
    /// <summary>
    /// Controller responsabile della gestione dei test disponibili.
    /// Fornisce endpoint per recuperare la lista dei test e i dettagli di test specifici.
    /// </summary>
    public class TestController
    {
        /// <summary>
        /// Servizio per l'accesso al database
        /// </summary>
        private readonly IDatabaseService _dbService;

        /// <summary>
        /// Costruttore del TestController.
        /// Inietta il servizio database tramite dependency injection.
        /// </summary>
        /// <param name="dbService">Istanza del servizio database che implementa IDatabaseService</param>
        public TestController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Gestisce le richieste HTTP per i test.
        /// Supporta GET /tests per recuperare tutti i test disponibili.
        /// </summary>
        /// <param name="context">Contesto della richiesta HTTP contenente request e response</param>
        public void HandleRequest(HttpListenerContext context)
        {
            var response = context.Response;
            string path = context.Request.Url?.AbsolutePath ?? "";

            try
            {
                // ============================================
                // GET /tests - Recupera tutti i test
                // ============================================
                if (context.Request.HttpMethod == "GET" && path == "/tests")
                {
                    // Recupera tutti i test dal database
                    var tests = _dbService.GetAllTests();
                    
                    // Serializza i test in JSON
                    string json = JsonSerializer.Serialize(tests);
                    
                    // Converte in bytes UTF-8
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    
                    // Imposta Content-Type e scrive la risposta
                    response.ContentType = "application/json";
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                // ============================================
                // Endpoint non trovato
                // ============================================
                else
                {
                    // Risponde con 404 Not Found
                    response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("{\"message\": \"Endpoint not found\"}");
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                // Gestione errori - Log e risposta al client
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Errore in TestController: {ex.Message}");
                Console.ResetColor();
                
                // Risponde con 500 Internal Server Error
                response.StatusCode = 500;
                byte[] buffer = Encoding.UTF8.GetBytes($"{{\"error\":\"{ex.Message}\"}}");
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                // Chiude sempre lo stream della risposta
                response.OutputStream.Close(); 
                response.Close();              
            }
        }

        /// <summary>
        /// Gestisce la richiesta per ottenere tutti i test.
        /// Recupera i test dal database e li invia come risposta JSON.
        /// </summary>
        /// <param name="context">Contesto HTTP della richiesta</param>
        private void HandleGetAllTests(HttpListenerContext context)
        {
            // Recupera tutti i test dal database service
            var tests = _dbService.GetAllTests();
            
            // Invia la risposta con status 200 OK
            SendResponse(context, 200, tests);
        }

        /// <summary>
        /// Gestisce la richiesta per ottenere un test specifico tramite ID.
        /// </summary>
        /// <param name="context">Contesto HTTP della richiesta</param>
        /// <param name="id">ID del test da recuperare</param>
        private void HandleGetTestById(HttpListenerContext context, string id)
        {
            // Recupera il test dal database usando l'ID
            var test = _dbService.GetTestById(id);
            
            // Se il test non esiste, risponde con 404
            if (test == null)
                SendResponse(context, 404, new { message = "Test not found" });
            else
                SendResponse(context, 200, test);
        }

        /// <summary>
        /// Invia una risposta HTTP in formato JSON.
        /// Metodo helper per standardizzare le risposte del controller.
        /// </summary>
        /// <param name="context">Contesto HTTP della richiesta</param>
        /// <param name="statusCode">Codice di stato HTTP (es. 200, 404, 500)</param>
        /// <param name="data">Oggetto da serializzare e inviare come JSON</param>
        private void SendResponse(HttpListenerContext context, int statusCode, object data)
        {
            // Serializza l'oggetto in formato JSON
            string json = JsonSerializer.Serialize(data);
            
            // Converte in array di bytes UTF-8
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            
            // Imposta status code e content type
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            
            // Scrive il buffer nello stream di output
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            
            // Chiude la connessione
            context.Response.Close();
        }
    }
}