using System.Net;
using System.Text;
using System.Text.Json;
using CogesQuizApp.Services;
using CogesQuizApp.Models;

namespace CogesQuizApp.Controllers
{
    public class ResultController
    {
        private readonly IDatabaseService _dbService;

        public ResultController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        public void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string path = request.Url.AbsolutePath;

            try
            {
                // ✅ Salva un nuovo risultato
                if (request.HttpMethod == "POST" && path.StartsWith("/results"))
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    string body = reader.ReadToEnd();
                    var result = JsonSerializer.Deserialize<Result>(body);
                    result.Date = DateTime.Now;
                    _dbService.SaveResult(result);

                    SendResponse(response, 200, new { message = "Result saved successfully" });
                }
                // ✅ Restituisce tutti i risultati
                else if (request.HttpMethod == "GET" && path.StartsWith("/results"))
                {
                    var results = _dbService.GetAllResults();
                    SendResponse(response, 200, results);
                }
                else
                {
                    SendResponse(response, 404, new { message = "Endpoint not found" });
                }
            }
            catch (Exception ex)
            {
                SendResponse(response, 500, new { error = ex.Message });
            }
        }

        private void SendResponse(HttpListenerResponse response, int statusCode, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.StatusCode = statusCode;
            response.ContentType = "application/json";
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }
    }
}
