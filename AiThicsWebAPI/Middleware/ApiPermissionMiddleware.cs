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
            //if (!context.User.Identity.IsAuthenticated)
            //{
            //    await _next(context); // ✅ Allow unauthenticated users to hit authentication endpoints
            //    return;
            //}

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var apiEndpoint = context.Request.Path.Value;

            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out long uid))
            {
                Console.WriteLine("❌ UserID is NULL, skipping permission check.");
                await _next(context);
                return;
            }

            var hasAccess = await permissionService.UserHasAccessAsync(uid, apiEndpoint);
            if (!hasAccess)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: You do not have access to this API.");
                return;
            }

            await _next(context);
        }


    }
}
