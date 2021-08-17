using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System.Threading.Tasks;

namespace ReceptTracker.Services
{
    public interface IAuthenticationService
    {
        GoogleUser GetUser();
        Task<GoogleActionStatus> LoginAsync();
        Task<bool> LogoutAsync();
    }
}
