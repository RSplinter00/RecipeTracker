using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System.Threading.Tasks;

namespace ReceptTracker.Controllers
{
    public interface IAuthenticationService
    {
        GoogleUser GetUser();
        Task<GoogleActionStatus> LoginAsync();
        bool Logout();
    }
}
