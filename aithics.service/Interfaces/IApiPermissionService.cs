using aithics.data.Models;

namespace aithics.service.Interfaces
{
    public interface IApiPermissionService
    {
        Task<bool> UserHasAccessAsync(long userId, string apiEndpoint);
    }
}
