using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace s17738_cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path;
                string queryString = httpContext.Request?.QueryString.ToString();
                string method = httpContext.Request.Method.ToString();

                string savePath = Directory.GetCurrentDirectory();
                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(savePath, "requestsLog.txt"), true, Encoding.UTF8))
                {
                    string body = await reader.ReadToEndAsync();
                    await outputFile.WriteLineAsync($"{method} {path} {queryString} {body}");
                }
            }

            await _next(httpContext);
        }
    }

}
