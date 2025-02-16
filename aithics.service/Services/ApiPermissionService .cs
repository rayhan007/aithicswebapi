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
            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == userId)
                                                    .Select(ur => ur.RoleId)
                                                    .ToListAsync();

            var permittedApis = await _context.RoleToAPIs
                                              .Where(x => userRoles.Contains(x.RoleId))
                                              .Select(x => x.APIListingId)
                                              .ToListAsync();

            // Returns true if there is any permitted API, false otherwise
            return permittedApis.Any();
        }
    }
}
