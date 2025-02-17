using aithics.service.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace aithics.api.Middleware
{
    public class ApiPermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiPermissionMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context, IApiPermissionService permissionService)
        {
            var apiEndpoint = context.Request.Path.Value.ToLower();

            // Exclude Login & Register APIs from Middleware
            if (apiEndpoint.Contains("/api/auth/login"))
            {
                await _next(context);
                return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && long.TryParse(userId, out long uid))
            {
                var hasAccess = await permissionService.UserHasAccessAsync(uid, apiEndpoint);
                if (!hasAccess)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: You do not have access to this API.");
                    return;
                }
            }

            await _next(context);
        }

    }
}
