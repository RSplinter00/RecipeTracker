using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    public interface IAuthenticationService
    {
        GoogleUser GetUser();
        Task<GoogleActionStatus> LoginAsync();
        Task<bool> LogoutAsync();
    }
}
