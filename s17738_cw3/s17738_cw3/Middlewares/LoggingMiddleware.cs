using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using s17738_cw3.DAL;

namespace s17738_cw3.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IDbService dbService)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path;
                string queryString = httpContext.Request?.QueryString.ToString();
                string method = httpContext.Request.Method.ToString();
                string body = "";

                using (StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    body = await reader.ReadToEndAsync();
                }

                string savePath = Directory.GetCurrentDirectory();
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(savePath, "requestsLog.txt"), true, Encoding.UTF8))
                {
                    outputFile.WriteLine($"{method} {path} {queryString} {body}");
                }
            }

            await _next(httpContext);
        }
    }

}
