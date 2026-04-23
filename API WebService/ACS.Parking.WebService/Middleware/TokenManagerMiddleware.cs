using ACS.Helper.V1;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ACS.Parking.WebService.Middleware
{
    public class TokenManagerMiddleware(ITokenService v1, FindClaimHelper findClaimHelper) : IMiddleware
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
            {
                await next(context);
                return;
            }

            if (context.Request.Path.Value == "/_health"
                || context.Request.Path.Value == "/graphql"
                || context.Request.Path.Value == "/ui/graphiql"
                || context.Request.Path.Value == "/ui/altair"
                || context.Request.Path.Value == "/ui/voyager")
            {
                await next(context);
                return;
            }

            string version = findClaimHelper.FindClaim(context, "ver");
            switch (version)
            {
                case "1":
                    var response = await v1.IsCurrentActiveToken(context, context.RequestAborted);
                    if (response == null || !response.IsValid)
                    {
                        await Reject(context,
                            StatusCodes.Status401Unauthorized,
                            response?.ErrorCode ?? "INVALID_SESSION");
                        return;
                    }
                    break;
                default:
                    await Reject(context, StatusCodes.Status400BadRequest, "INVALID_API_VERSION");
                    break;
            }

            await next(context);
        }


        private static async Task Reject(HttpContext context, int statusCode, string errorCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error_code = errorCode,
                request_id = context.Items["X-Request-ID"]
            });
        }

    }
}