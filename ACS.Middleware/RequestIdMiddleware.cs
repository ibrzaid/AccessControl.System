using Microsoft.AspNetCore.Http;

namespace ACS.Middleware
{
    public class RequestIdMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("X-Request-ID"))
            {
                var requestId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                context.Request.Headers.Add("X-Request-ID", requestId);
            }
            await next(context);
        }
    }
}
