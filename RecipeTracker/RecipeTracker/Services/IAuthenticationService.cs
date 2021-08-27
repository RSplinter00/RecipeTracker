using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    /// <summary>
    /// Interface <c>IAuthenticationService</c> is the template used for services that deal with authenticating the user.
    /// 
    /// <para>
    ///     For Google Authentication, see <seealso cref="GoogleAuthenticationService"/>.
    /// </para>
    /// </summary>
    public interface IAuthenticationService
    {
        GoogleUser GetUser();
        Task<GoogleActionStatus> LoginAsync();
        Task<bool> LogoutAsync();
    }
}
