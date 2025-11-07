using System;
using System.IO;
using System.Net;
using System.Text;
using CogesQuizApp.Services;
using CogesQuizApp.Controllers;

namespace CogesQuizApp
{
    /// <summary>
    /// Classe principale dell'applicazione Coges Quiz App.
    /// Configura e avvia il server HTTP per servire l'applicazione web e gestire le API REST.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Punto di ingresso dell'applicazione.
        /// Inizializza i servizi, configura i controller e avvia il server HTTP.
        /// </summary>
        /// <param name="args">Argomenti della linea di comando (non utilizzati)</param>
        static void Main(string[] args)
        {
            // ============================================
            // Configurazione Database
            // ============================================
            
            // Connection string per MongoDB locale
            string connectionString = "mongodb://localhost:27017";
            
            // Nome del database da utilizzare
            string databaseName = "CogesQuizDB";

            // ============================================
            // Inizializzazione Servizi e Controller
            // ============================================
            
            // Crea l'istanza del servizio database (implementa IDatabaseService)
            var dbService = new DatabaseService(connectionString, databaseName);
            
            // Inizializza i controller con dependency injection del database service
            var testController = new TestController(dbService);
            var resultController = new ResultController(dbService);
            var userAnswerController = new UserAnswerController(dbService);

            // ============================================
            // Configurazione Path wwwroot
            // ============================================
            
            // Calcola il percorso assoluto della cartella wwwroot
            // Risale di 3 livelli dalla cartella bin/Debug/net9.0
            string webRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot");
            webRoot = Path.GetFullPath(webRoot);

            // Verifica che la cartella wwwroot esista
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

            // ============================================
            // Configurazione e Avvio Server HTTP
            // ============================================
            
            // Crea e configura l'HttpListener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            // Stampa informazioni di avvio
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

            // ============================================
            // Loop Principale del Server
            // ============================================
            
            // Loop infinito per gestire le richieste HTTP
            while (true)
            {
                try
                {
                    // Attende una richiesta in arrivo (operazione bloccante)
                    var context = listener.GetContext();
                    
                    // Estrae il path dalla richiesta
                    string path = context.Request.Url?.AbsolutePath ?? "/";
                    string method = context.Request.HttpMethod;

                    // Log della richiesta ricevuta
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"➡️  {method} {path}");
                    Console.ResetColor();

                    // ============================================
                    // Routing per File Statici
                    // ============================================
                    
                    // Tenta di servire un file statico (HTML, CSS, JS, immagini)
                    if (ServeStaticFile(context, webRoot, path))
                    {
                        // File statico servito con successo, continua con la prossima richiesta
                        continue;
                    }

                    // ============================================
                    // Routing per API Endpoints
                    // ============================================
                    
                    // Route: /tests - Gestisce operazioni sui test
                    if (path.StartsWith("/tests"))
                    {
                        testController.HandleRequest(context);
                    }
                    // Route: /results - Gestisce operazioni sui risultati
                    else if (path.StartsWith("/results"))
                    {
                        resultController.HandleRequest(context);
                    }
                    // Route: /user-answers - Gestisce operazioni sulle risposte utente
                    else if (path.StartsWith("/user-answers"))
                    {
                        userAnswerController.HandleRequest(context);
                    }
                    // Endpoint non trovato
                    else
                    {
                        // Risponde con 404 Not Found
                        context.Response.StatusCode = 404;
                        byte[] buffer = Encoding.UTF8.GetBytes("{\"message\": \"Endpoint not found\"}");
                        context.Response.ContentType = "application/json";
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    // Gestisce eventuali errori durante l'elaborazione delle richieste
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Errore nel server: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Gestisce il serving di file statici (HTML, CSS, JavaScript, immagini).
        /// Cerca il file nel filesystem e lo invia al client se trovato.
        /// </summary>
        /// <param name="context">Contesto della richiesta HTTP</param>
        /// <param name="webRoot">Path della cartella wwwroot contenente i file statici</param>
        /// <param name="path">Path richiesto dal client</param>
        /// <returns>True se il file è stato trovato e servito, False altrimenti</returns>
        private static bool ServeStaticFile(HttpListenerContext context, string webRoot, string path)
        {
            // Determina il percorso del file richiesto
            string filePath;
            
            // Se è la root ("/"), serve index.html
            if (path == "/" || path == "")
            {
                filePath = Path.Combine(webRoot, "index.html");
            }
            else
            {
                // Rimuove lo slash iniziale e converte in path del filesystem
                filePath = Path.Combine(webRoot, path.TrimStart('/')
                    .Replace("/", Path.DirectorySeparatorChar.ToString()));
            }

            // Verifica se il file esiste
            if (File.Exists(filePath))
            {
                // Determina il Content-Type in base all'estensione del file
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

                // Legge il contenuto del file
                byte[] content = File.ReadAllBytes(filePath);
                
                // Imposta gli headers della risposta con encoding UTF-8
                context.Response.ContentType = contentType + "; charset=utf-8";
                context.Response.ContentLength64 = content.Length;
                context.Response.ContentEncoding = Encoding.UTF8;
                
                // Scrive il contenuto nella risposta
                context.Response.OutputStream.Write(content, 0, content.Length);
                context.Response.Close();
                
                return true;
            }

            // File non trovato
            return false;
        }
    }
}