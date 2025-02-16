using aithics.data.Models;

namespace aithics.service.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(User user, string password);
        Task<string> LoginAsync(string email, string password);
    }
}
