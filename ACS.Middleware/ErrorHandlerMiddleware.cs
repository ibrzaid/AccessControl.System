using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace ACS.Middleware
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        /// <summary>
        /// Invoke Logs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "ERR01" 
                };

                string jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400; // You can customize this based on ex type
                await context.Response.WriteAsync(jsonString);
            }
        }
    }
}

