using Newtonsoft.Json;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReceptTracker.Controllers
{
    public class GoogleAuthenticationService : IAuthenticationService
    {
        private IGoogleClientManager googleService = CrossGoogleClient.Current;
        public GoogleUser GetUser() => googleService.CurrentUser;

        public async Task<GoogleActionStatus> LoginAsync()
        {
            try
            {
                if (googleService.IsLoggedIn)
                {
                    return GoogleActionStatus.Completed;
                }

                if (!string.IsNullOrEmpty(googleService.AccessToken))
                {
                    // Always require user authentication
                    googleService.Logout();
                }

                GoogleActionStatus status = GoogleActionStatus.Unauthorized;

                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                userLoginDelegate = (object sender, GoogleClientResultEventArgs<GoogleUser> e) =>
                {
                    status = e.Status;
#if DEBUG
                    var googleUserString = JsonConvert.SerializeObject(e.Data);
                    Debug.WriteLine($"Google Login attempt by {googleUserString}: {e.Status}");
#endif
                    googleService.OnLogin -= userLoginDelegate;
                };

                googleService.OnLogin += userLoginDelegate;
                await googleService.LoginAsync();

                return status;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return GoogleActionStatus.Error;
            }
        }

        public bool Logout()
        {
            try
            {
                googleService.Logout();

            }
            catch (Exception)
            {
            }

            return !googleService.IsLoggedIn;
        }
    }
}
