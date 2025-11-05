using System;
using System.IO;
using System.Net;
using System.Text;
using CogesQuizApp.Services;
using CogesQuizApp.Controllers;

namespace CogesQuizApp
{
    /// <summary>
    /// Classe principale dell'applicazione.
    /// Configura e avvia il server HTTP per servire l'applicazione web.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configurazione database
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "CogesQuizDB";

            // Inizializzazione servizi e controller
            var dbService = new DatabaseService(connectionString, databaseName);
            var testController = new TestController(dbService);
            var resultController = new ResultController(dbService);
            var userAnswerController = new UserAnswerController(dbService);

            // Percorso assoluto della cartella wwwroot
            string webRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot");
            webRoot = Path.GetFullPath(webRoot);

            // Verifica esistenza cartella wwwroot
            if (!Directory.Exists(webRoot))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Errore: la cartella wwwroot non è stata trovata in {webRoot}");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Cartella wwwroot trovata in: {webRoot}");
            Console.ResetColor();

            // Avvia il server HTTP
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🚀 Server avviato su http://localhost:8080/");
            Console.ResetColor();
            Console.WriteLine("👉 Endpoints API disponibili:");
            Console.WriteLine("   GET  /tests              - Ottieni tutti i test");
            Console.WriteLine("   GET  /results            - Ottieni tutti i risultati");
            Console.WriteLine("   POST /results            - Salva un risultato");
            Console.WriteLine("   POST /user-answers       - Salva una risposta singola");
            Console.WriteLine("   GET  /user-answers/session/{id} - Risposte per sessione");
            Console.WriteLine("-----------------------------------------");

            // Loop principale del server
            while (true)
            {
                try
                {
                    var context = listener.GetContext();
                    string path = context.Request.Url?.AbsolutePath ?? "/";
                    string method = context.Request.HttpMethod;

                    // Log della richiesta
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"➡️  {method} {path}");
                    Console.ResetColor();

                    // Routing per file statici
                    if (ServeStaticFile(context, webRoot, path))
                    {
                        continue;
                    }

                    // Routing per API endpoints
                    if (path.StartsWith("/tests"))
                    {
                        testController.HandleRequest(context);
                    }
                    else if (path.StartsWith("/results"))
                    {
                        resultController.HandleRequest(context);
                    }
                    else if (path.StartsWith("/user-answers"))
                    {
                        userAnswerController.HandleRequest(context);
                    }
                    else
                    {
                        // Endpoint non trovato
                        context.Response.StatusCode = 404;
                        byte[] buffer = Encoding.UTF8.GetBytes("{\"message\": \"Endpoint not found\"}");
                        context.Response.ContentType = "application/json";
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Errore nel server: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Gestisce il serving di file statici (HTML, CSS, JS, immagini)
        /// </summary>
        /// <param name="context">Contesto HTTP</param>
        /// <param name="webRoot">Path della cartella wwwroot</param>
        /// <param name="path">Path richiesto</param>
        /// <returns>True se il file è stato servito, False altrimenti</returns>
        private static bool ServeStaticFile(HttpListenerContext context, string webRoot, string path)
        {
            // Se root, serve index.html
            string filePath;
            if (path == "/" || path == "")
            {
                filePath = Path.Combine(webRoot, "index.html");
            }
            else
            {
                filePath = Path.Combine(webRoot, path.TrimStart('/')
                    .Replace("/", Path.DirectorySeparatorChar.ToString()));
            }

            // Se il file esiste, lo serviamo
            if (File.Exists(filePath))
            {
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                string contentType = ext switch
                {
                    ".html" => "text/html",
                    ".css" => "text/css",
                    ".js" => "application/javascript",
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".svg" => "image/svg+xml",
                    ".ico" => "image/x-icon",
                    ".json" => "application/json",
                    _ => "text/plain"
                };

                byte[] content = File.ReadAllBytes(filePath);
                context.Response.ContentType = contentType;
                context.Response.ContentLength64 = content.Length;
                context.Response.OutputStream.Write(content, 0, content.Length);
                context.Response.Close();
                return true;
            }

            return false;
        }
    }
}