using System;
using System.IO;
using System.Net;
using System.Text;
using CogesQuizApp.Services;
using CogesQuizApp.Controllers;

namespace CogesQuizApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "CogesQuizDB";

            var dbService = new DatabaseService(connectionString, databaseName);
            var testController = new TestController(dbService);
            var resultController = new ResultController(dbService);

            // ✅ Percorso assoluto della cartella wwwroot
            string webRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wwwroot");
            webRoot = Path.GetFullPath(webRoot);

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
            Console.WriteLine("👉 Endpoints disponibili:");
            Console.WriteLine("   GET /tests");
            Console.WriteLine("   GET /results");
            Console.WriteLine("-----------------------------------------");

            while (true)
            {
                var context = listener.GetContext();
                string path = context.Request.Url?.AbsolutePath ?? "/";

                // ✅ Log utile per debug
                Console.WriteLine($"➡️  Richiesta ricevuta: {path}");

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

                // ✅ Se il file statico esiste → lo serviamo
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
                        ".ico" => "image/x-icon",
                        _ => "text/plain"
                    };

                    byte[] content = File.ReadAllBytes(filePath);
                    context.Response.ContentType = contentType;
                    context.Response.OutputStream.Write(content, 0, content.Length);
                    context.Response.Close();
                    continue;
                }

                // ✅ Altrimenti controlla gli endpoint API
                if (path.StartsWith("/tests"))
                {
                    testController.HandleRequest(context);
                }
                else if (path.StartsWith("/results"))
                {
                    resultController.HandleRequest(context);
                }
                else
                {
                    // ❌ Endpoint o file non trovati
                    context.Response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("{\"message\": \"Endpoint not found\"}");
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.Close();
                }
            }
        }
    }
}
