using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReceptTracker.Services
{
    public class GoogleAuthenticationService : IAuthenticationService
    {
        private readonly IGoogleClientManager GoogleService = CrossGoogleClient.Current;
        public GoogleUser GetUser() => GoogleService.CurrentUser;
        public static string UserID { get; private set; } = "";

        private static FirebaseClient firebase;
        public FirebaseClient Firebase
        {
            get
            {
                if (firebase == null) SetupFirebaseClient().Wait();

                return firebase;
            }
            private set => firebase = value;
        }

        private async Task SetupFirebaseClient()
        {
            FirebaseOptions options = null;
            if (!string.IsNullOrEmpty(GoogleService.AccessToken))
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettingsManager.Settings["AndroidAPIKey"]));
                var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, GoogleService.AccessToken);
                
                UserID = auth.User.LocalId;
                options = new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                };
            }

            Firebase = new FirebaseClient(AppSettingsManager.Settings["FirebaseDatabasePath"], options);
        }

        public async Task<GoogleActionStatus> LoginAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(GoogleService.AccessToken))
                {
                    // Always require user authentication
                    GoogleService.Logout();
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
                    GoogleService.OnLogin -= userLoginDelegate;
                };

                GoogleService.OnLogin += userLoginDelegate;
                await GoogleService.LoginAsync();

                if (status == GoogleActionStatus.Completed) await SetupFirebaseClient();

                return status;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return GoogleActionStatus.Error;
            }
        }

        public async Task<bool> Logout()
        {
            try
            {
                GoogleService.Logout();
                await SetupFirebaseClient();

            }
            catch (Exception)
            {
            }

            return !GoogleService.IsLoggedIn;
        }
    }
}
