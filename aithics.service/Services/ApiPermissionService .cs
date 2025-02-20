using aithics.data;
using aithics.service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace aithics.service.Services
{
    public class ApiPermissionService : IApiPermissionService
    {
        private readonly AithicsDbContext _context;

        public ApiPermissionService(AithicsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasAccessAsync(long userId, string apiEndpoint)
        {
            try
            {
                // Get RoleId for the user
                long? roleId = await _context.UserRoles
                    .Where(x => x.UserId == userId)
                    .Select(x => x.RoleId)
                    .FirstOrDefaultAsync(); // Returns default value (null) instead of throwing an exception

                if (roleId == null)
                {
                    return false; // No role found for the user
                }

                // Get API Listing ID for the requested API Endpoint
                long? apiListingId = await _context.APIlistings
                    .Where(x => x.ApiEndpoint == apiEndpoint)
                    .Select(x => x.APIListingId)
                    .FirstOrDefaultAsync(); // Returns null if no match is found

                if (apiListingId == null)
                {
                    return false; // No API mapping found for the given endpoint
                }

                // Check if Role has permission for this API
                bool hasAccess = await _context.RoleToAPIs
                    .AnyAsync(x => x.RoleId == roleId && x.APIListingId == apiListingId);

                return hasAccess;
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logging framework like Serilog)
                Console.WriteLine($"Error in UserHasAccessAsync: {ex.Message}");
                return false; // Return false in case of any exception
            }
        }

    }
}
