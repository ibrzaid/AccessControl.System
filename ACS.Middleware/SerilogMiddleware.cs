using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.Json;



namespace ACS.Middleware
{
    /// <summary>
    /// Serilog Middleware
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public class SerilogMiddleware(RequestDelegate next, ILogger<SerilogMiddleware> logger)
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task Invoke(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            var stopwatch = new Stopwatch();


            string requestTime = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss.fff");
            HttpRequestRewindExtensions.EnableBuffering(httpContext.Request);
            Stream body = httpContext.Request.Body;
            byte[] buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength)];
            await httpContext.Request.Body.ReadAsync(buffer);
            string requestBody = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            httpContext.Request.Body = body;

            var RemoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var requestHeaders = JsonSerializer.Serialize(httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            using var responseBodyMemoryStream = new MemoryStream();
            var originalResponseBodyReference = httpContext.Response.Body;
            httpContext.Response.Body = responseBodyMemoryStream;

            stopwatch.Start();

            await next(httpContext);

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            string responseTime = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss.fff");
            var responseHeaders = JsonSerializer.Serialize(httpContext.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            logger.LogInformation("RequestPath: {RequestPath}, RequestTime: {RequestTime}, ResponseTime: {ResponseTime}, ElapsedTimeMs: {ElapsedTimeMs},  RequestHeaders: {RequestHeaders}, RequestBody: {RequestBody}, ResponseStatusCode: {ResponseStatusCode}, ResponseHeaders : {ResponseHeaders}, ResponseBody: {ResponseBody}, RemoteIpAddress :{RemoteIpAddress}",
                httpContext.Request.Path, requestTime, responseTime, elapsedMs, requestHeaders, requestBody, httpContext.Response.StatusCode, responseHeaders, responseBody, RemoteIpAddress);

            await responseBodyMemoryStream.CopyToAsync(originalResponseBodyReference);

        }
    }
}
