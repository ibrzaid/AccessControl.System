using System.Security.Claims;

namespace ACS.Parking.WebService.GraphQL
{
    public class GraphQLUserContext(HttpContext httpContext) : Dictionary<string, object?>
    {
        public ClaimsPrincipal User { get; } = httpContext.User;
        public HttpContext HttpContext { get; } = httpContext;

        public int? WorkspaceId
        {
            get
            {
                var workspaceClaim = User.FindFirst("workspace_id") ??
                                   User.FindFirst("workspace") ??
                                   User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/workspace");

                if (int.TryParse(workspaceClaim?.Value, out int workspaceId))
                    return workspaceId;

                return null;
            }
        }

        public string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? Username => User.FindFirst(ClaimTypes.Name)?.Value;
        public string? Email => User.FindFirst(ClaimTypes.Email)?.Value;
    }
}
