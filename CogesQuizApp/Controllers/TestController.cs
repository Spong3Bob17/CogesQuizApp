using System;
using System.Net;
using System.Text;
using System.Text.Json;
using CogesQuizApp.Services;

namespace CogesQuizApp.Controllers
{
    public class TestController
    {
        private readonly IDatabaseService _dbService;

        public TestController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }

        // Gestisce richieste GET /tests e GET /tests/{id}
        public void HandleRequest(HttpListenerContext context)
        {
            var response = context.Response;
            string path = context.Request.Url?.AbsolutePath ?? "";

            try
            {
                if (context.Request.HttpMethod == "GET" && path == "/tests")
                {
                    var tests = _dbService.GetAllTests();
                    string json = JsonSerializer.Serialize(tests);
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    response.ContentType = "application/json";
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("{\"message\": \"Endpoint not found\"}");
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                byte[] buffer = Encoding.UTF8.GetBytes($"{{\"error\":\"{ex.Message}\"}}");
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                response.OutputStream.Close(); 
                response.Close();              
            }
        }


        private void HandleGetAllTests(HttpListenerContext context)
        {
            var tests = _dbService.GetAllTests();
            SendResponse(context, 200, tests);
        }

        private void HandleGetTestById(HttpListenerContext context, string id)
        {
            var test = _dbService.GetTestById(id);
            if (test == null)
                SendResponse(context, 404, new { message = "Test not found" });
            else
                SendResponse(context, 200, test);
        }

        private void SendResponse(HttpListenerContext context, int statusCode, object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}
