using System.Net;
using ACS.Helper.V1;
using ACS.Service.V1.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace ACS.Setup.WebService.Middleware
{
    /// <summary>
    /// Token Manager Middleware
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="findClaimHelper"></param>
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

            if (context.Request.Path.Value == "/_health")
            {
                await next(context);
                return;
            }

            //string version = findClaimHelper.FindClaim(context, "version");
            //bool status = version switch
            //{
            //    _ => await v1.IsCurrentActiveToken(context, context.RequestAborted),
            //};

            //if (status)
            //{
            await next(context);
            //    return;
            //}

            //context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
