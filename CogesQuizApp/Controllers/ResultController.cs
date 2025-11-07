using System.Net;
using System.Text;
using System.Text.Json;
using CogesQuizApp.Services;
using CogesQuizApp.Models;

namespace CogesQuizApp.Controllers
{
    /// <summary>
    /// Controller responsabile della gestione dei risultati dei quiz.
    /// Gestisce operazioni di salvataggio e recupero dei risultati finali.
    /// </summary>
    public class ResultController
    {
        /// <summary>
        /// Servizio per l'accesso al database
        /// </summary>
        private readonly IDatabaseService _dbService;

        /// <summary>
        /// Costruttore del ResultController.
        /// Inietta il servizio database tramite dependency injection.
        /// </summary>
        /// <param name="dbService">Istanza del servizio database</param>
        public ResultController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Gestisce le richieste HTTP per i risultati.
        /// Supporta operazioni GET (recupero risultati) e POST (salvataggio risultato).
        /// </summary>
        /// <param name="context">Contesto della richiesta HTTP contenente request e response</param>
        public void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string path = request.Url.AbsolutePath;

            try
            {
                // ============================================
                // POST /results - Salva un nuovo risultato
                // ============================================
                if (request.HttpMethod == "POST" && path.StartsWith("/results"))
                {
                    // Legge il body della richiesta
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    string body = reader.ReadToEnd();
                    
                    // Deserializza il JSON in un oggetto Result
                    var result = JsonSerializer.Deserialize<Result>(body);
                    
                    // Imposta la data corrente se non specificata
                    result.Date = DateTime.Now;
                    
                    // Salva nel database
                    _dbService.SaveResult(result);

                    // Risponde con successo
                    SendResponse(response, 200, new { message = "Result saved successfully" });
                }
                // ============================================
                // GET /results - Recupera tutti i risultati
                // ============================================
                else if (request.HttpMethod == "GET" && path.StartsWith("/results"))
                {
                    // Recupera tutti i risultati dal database (ordinati per data)
                    var results = _dbService.GetAllResults();
                    
                    // Invia i risultati come JSON
                    SendResponse(response, 200, results);
                }
                // ============================================
                // Endpoint non trovato
                // ============================================
                else
                {
                    SendResponse(response, 404, new { message = "Endpoint not found" });
                }
            }
            catch (Exception ex)
            {
                // Gestione errori - Log e risposta al client
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Errore in ResultController: {ex.Message}");
                Console.ResetColor();
                
                SendResponse(response, 500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Invia una risposta HTTP in formato JSON.
        /// Serializza l'oggetto data in JSON e lo scrive nella response stream.
        /// </summary>
        /// <param name="response">Oggetto HttpListenerResponse per inviare la risposta</param>
        /// <param name="statusCode">Codice di stato HTTP (es. 200, 404, 500)</param>
        /// <param name="data">Oggetto da serializzare in JSON</param>
        private void SendResponse(HttpListenerResponse response, int statusCode, object data)
        {
            // Opzioni di serializzazione con supporto UTF-8 per caratteri accentati
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            
            // Serializza l'oggetto in JSON
            string json = JsonSerializer.Serialize(data, options);
            
            // Converte la stringa JSON in bytes UTF-8
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            
            // Imposta gli headers della risposta con charset UTF-8
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";
            response.ContentEncoding = Encoding.UTF8;
            
            // Scrive il contenuto nella response stream
            response.OutputStream.Write(buffer, 0, buffer.Length);
            
            // Chiude la risposta
            response.Close();
        }
    }
}